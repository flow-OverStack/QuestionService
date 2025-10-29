using QuestionService.Domain.Enums;

namespace QuestionService.Application.Mappings;

public static class VoteEventMapper
{
    private static readonly IReadOnlyDictionary<VoteTypes, BaseEventType> Mapping =
        new Dictionary<VoteTypes, BaseEventType>
        {
            [VoteTypes.Upvote] = BaseEventType.QuestionUpvote,
            [VoteTypes.Downvote] = BaseEventType.QuestionDownvote
        };

    public static BaseEventType Map(string voteTypeString)
    {
        if (!System.Enum.TryParse<VoteTypes>(voteTypeString, out var voteType))
            throw new InvalidOperationException($"Unknown VoteType: {voteTypeString}");

        if (!Mapping.TryGetValue(voteType, out var eventType))
            throw new InvalidOperationException($"No mapping for VoteType {voteType}");

        return eventType;
    }
}