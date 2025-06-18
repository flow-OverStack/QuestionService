using Microsoft.Extensions.Options;
using QuestionService.Domain.Dtos.Vote;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Extensions;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;
using QuestionService.Domain.Settings;
using StackExchange.Redis;

namespace QuestionService.Application.Services.Cache;

public class CacheGetVoteService(
    GetVoteService inner,
    IDatabase redisDatabase,
    IOptions<RedisSettings> redisSettings) : IGetVoteService
{
    private const string VoteValuePattern = "{0},{1}";

    private readonly RedisSettings _redisSettings = redisSettings.Value;

    public Task<QueryableResult<Vote>> GetAllAsync(CancellationToken cancellationToken = default) =>
        inner.GetAllAsync(cancellationToken);

    public async Task<CollectionResult<Vote>> GetByDtosAsync(IEnumerable<GetVoteDto> dtos,
        CancellationToken cancellationToken = default)
    {
        var idsList = dtos.ToList();

        try
        {
            var keys = idsList.Select(x => RedisKeyHelper.GetVoteKey(x.QuestionId, x.UserId));
            var votes = (await redisDatabase.GetJsonParsedAsync<Vote>(keys, cancellationToken)).ToList();

            var missingIds = idsList.Except(votes.Select(x => new GetVoteDto(x.QuestionId, x.UserId))).ToList();

            if (missingIds.Count > 0) return await GetFromInnerAndCacheAsync(missingIds, votes);

            return CollectionResult<Vote>.Success(votes);
        }
        catch (Exception)
        {
            return await GetFromInnerAndCacheAsync(idsList, []);
        }

        async Task<CollectionResult<Vote>> GetFromInnerAndCacheAsync(IEnumerable<GetVoteDto> missingIds,
            IEnumerable<Vote> alreadyCached)
        {
            var alreadyCachedList = alreadyCached.ToList();

            var result = await inner.GetByDtosAsync(missingIds, cancellationToken);

            if (!result.IsSuccess)
                return alreadyCachedList.Count > 0
                    ? CollectionResult<Vote>.Success(alreadyCachedList)
                    : result;

            var allVotes = result.Data.UnionBy(alreadyCachedList, x => new { x.QuestionId, x.UserId }).ToList();

            var keyVotes = allVotes.Select(x =>
                new KeyValuePair<string, Vote>(RedisKeyHelper.GetVoteKey(x.QuestionId, x.UserId), x));

            await redisDatabase.StringSetAsync(keyVotes, _redisSettings.TimeToLiveInSeconds,
                cancellationToken);

            return CollectionResult<Vote>.Success(allVotes);
        }
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>> GetQuestionsVotesAsync(
        IEnumerable<long> questionIds, CancellationToken cancellationToken = default)
    {
        var idsList = questionIds.ToList();

        try
        {
            var questionVoteKeys = idsList.Select(RedisKeyHelper.GetQuestionVotesKey);
            var questionVoteStringIds =
                (await redisDatabase.SetsStringMembersAsync(questionVoteKeys, cancellationToken)).Where(x =>
                    x.Value.Any());

            var questionVoteIds = questionVoteStringIds.Select(x =>
                new KeyValuePair<long, IEnumerable<GetVoteDto>>(RedisKeyHelper.GetIdFromKey(x.Key),
                    x.Value.Select(v =>
                    {
                        var parts = GetIdsFromValue(v);
                        return new GetVoteDto(parts[0], parts[1]);
                    }))).ToList();

            var voteKeys = questionVoteIds
                .SelectMany(x => x.Value.Select(v => RedisKeyHelper.GetVoteKey(v.QuestionId, v.UserId)))
                .Distinct();
            var votes = await redisDatabase.GetJsonParsedAsync<Vote>(voteKeys, cancellationToken);

            var questionVotes = questionVoteIds.Select(kvp =>
                    new KeyValuePair<long, IEnumerable<Vote>>(
                        kvp.Key,
                        kvp.Value
                            .Select(v =>
                                votes.FirstOrDefault(vote =>
                                    vote.QuestionId == v.QuestionId && vote.UserId == v.UserId))
                            .Where(v => v != null)!))
                .ToList();

            var missingQuestionVotes = idsList.Except(questionVoteIds.Select(x => x.Key)).Distinct().ToList();
            var cachedQuestionVotes = new List<KeyValuePair<long, IEnumerable<Vote>>>();
            foreach (var questionVoteId in questionVoteIds)
            {
                // Keys in questionVoteIds are guaranteed to be in questionVotes

                var actualQuestionVote = questionVotes.First(x => x.Key == questionVoteId.Key);

                if (questionVoteId.Value
                    .Except(actualQuestionVote.Value.Select(x => new GetVoteDto(x.QuestionId, x.UserId))).Any())
                    missingQuestionVotes.Add(questionVoteId.Key);
                else
                    cachedQuestionVotes.Add(actualQuestionVote);
            }

            if (missingQuestionVotes.Count > 0)
                return await GetFromInnerAndCacheAsync(missingQuestionVotes, cachedQuestionVotes);

            return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Success(questionVotes);
        }
        catch (Exception)
        {
            return await GetFromInnerAndCacheAsync(idsList, []);
        }

        async Task<CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>> GetFromInnerAndCacheAsync(
            IEnumerable<long> missingIds, IEnumerable<KeyValuePair<long, IEnumerable<Vote>>> alreadyCached)
        {
            var alreadyCachedList = alreadyCached.ToList();

            var result = await inner.GetQuestionsVotesAsync(missingIds, cancellationToken);

            if (!result.IsSuccess)
                return alreadyCachedList.Count > 0
                    ? CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Success(alreadyCachedList)
                    : result;

            var allQuestionVotes = result.Data.UnionBy(alreadyCachedList, x => x.Key).ToList();

            var questionVoteStringIds = allQuestionVotes.Select(kvp =>
                new KeyValuePair<string, IEnumerable<string>>(RedisKeyHelper.GetQuestionVotesKey(kvp.Key),
                    kvp.Value.Select(x => GetVoteValue(x.QuestionId, x.UserId))));

            var votes = allQuestionVotes.SelectMany(x => x.Value);
            var voteKeys = votes.Select(x =>
                new KeyValuePair<string, Vote>(RedisKeyHelper.GetVoteKey(x.QuestionId, x.UserId), x));

            await redisDatabase.StringSetAsync(voteKeys, _redisSettings.TimeToLiveInSeconds,
                cancellationToken);
            await redisDatabase.SetsAddAsync(questionVoteStringIds, _redisSettings.TimeToLiveInSeconds,
                cancellationToken);

            return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Success(allQuestionVotes);
        }
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>> GetUsersVotesAsync(
        IEnumerable<long> userIds,
        CancellationToken cancellationToken = default)
    {
        var idsList = userIds.ToList();

        try
        {
            var userVoteKeys = idsList.Select(RedisKeyHelper.GetUserVotesKey);
            var questionVoteStringIds =
                (await redisDatabase.SetsStringMembersAsync(userVoteKeys, cancellationToken)).Where(x =>
                    x.Value.Any());

            var userVoteIds = questionVoteStringIds.Select(x =>
                new KeyValuePair<long, IEnumerable<GetVoteDto>>(RedisKeyHelper.GetIdFromKey(x.Key),
                    x.Value.Select(v =>
                    {
                        var parts = GetIdsFromValue(v);
                        return new GetVoteDto(parts[0], parts[1]);
                    }))).ToList();

            var voteKeys = userVoteIds
                .SelectMany(x => x.Value.Select(v => RedisKeyHelper.GetVoteKey(v.QuestionId, v.UserId)))
                .Distinct();
            var votes = await redisDatabase.GetJsonParsedAsync<Vote>(voteKeys, cancellationToken);

            var userVotes = userVoteIds.Select(kvp =>
                    new KeyValuePair<long, IEnumerable<Vote>>(
                        kvp.Key,
                        kvp.Value
                            .Select(v =>
                                votes.FirstOrDefault(vote =>
                                    vote.QuestionId == v.QuestionId && vote.UserId == v.UserId))
                            .Where(v => v != null)!))
                .ToList();

            var missingUserVotes = idsList.Except(userVoteIds.Select(x => x.Key)).Distinct().ToList();
            var cachedUserVotes = new List<KeyValuePair<long, IEnumerable<Vote>>>();
            foreach (var questionVoteId in userVoteIds)
            {
                // Keys in userVoteIds are guaranteed to be in userVotes

                var actualUserVote = userVotes.First(x => x.Key == questionVoteId.Key);

                if (questionVoteId.Value
                    .Except(actualUserVote.Value.Select(x => new GetVoteDto(x.QuestionId, x.UserId))).Any())
                    missingUserVotes.Add(questionVoteId.Key);
                else
                    cachedUserVotes.Add(actualUserVote);
            }

            if (missingUserVotes.Count > 0)
                return await GetFromInnerAndCacheAsync(missingUserVotes, cachedUserVotes);

            return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Success(userVotes);
        }
        catch (Exception)
        {
            return await GetFromInnerAndCacheAsync(idsList, []);
        }

        async Task<CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>> GetFromInnerAndCacheAsync(
            IEnumerable<long> missingIds, IEnumerable<KeyValuePair<long, IEnumerable<Vote>>> alreadyCached)
        {
            var alreadyCachedList = alreadyCached.ToList();

            var result = await inner.GetUsersVotesAsync(missingIds, cancellationToken);

            if (!result.IsSuccess)
                return alreadyCachedList.Count > 0
                    ? CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Success(alreadyCachedList)
                    : result;

            var allUserVotes = result.Data.UnionBy(alreadyCachedList, x => x.Key).ToList();

            var userVoteStringIds = allUserVotes.Select(kvp =>
                new KeyValuePair<string, IEnumerable<string>>(RedisKeyHelper.GetUserVotesKey(kvp.Key),
                    kvp.Value.Select(x => GetVoteValue(x.QuestionId, x.UserId))));

            var votes = allUserVotes.SelectMany(x => x.Value);
            var voteKeys = votes.Select(x =>
                new KeyValuePair<string, Vote>(RedisKeyHelper.GetVoteKey(x.QuestionId, x.UserId), x));

            await redisDatabase.StringSetAsync(voteKeys, _redisSettings.TimeToLiveInSeconds,
                cancellationToken);
            await redisDatabase.SetsAddAsync(userVoteStringIds, _redisSettings.TimeToLiveInSeconds,
                cancellationToken);

            return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Success(allUserVotes);
        }
    }

    private static string GetVoteValue(long questionId, long userId) =>
        string.Format(VoteValuePattern, questionId, userId);

    private static long[] GetIdsFromValue(string value)
    {
        var parts = value.Split(',');
        if (parts.Length < 2)
            throw new ArgumentException($"Invalid value format: {value}");

        return parts.Select(long.Parse).ToArray();
    }
}