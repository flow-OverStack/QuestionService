namespace QuestionService.Domain.Helpers;

public static class RedisKeyHelper
{
    private const string QuestionKeyPattern = "question:{0}";
    private const string TagQuestionsKeyPattern = "tag:{0}:questions";
    private const string UserQuestionsKeyPattern = "user:{0}:questions";

    private const string TagKeyPattern = "tag:{0}";
    private const string QuestionTagsKeyPattern = "question:{0}:tags";

    private const string ViewKeyPattern = "view:{0}";
    private const string UserViewsKeyPattern = "user:{0}:views";
    private const string QuestionViewsKeyPattern = "question:{0}:views";

    private const string VoteKeyPattern = "vote:{0},{1}";
    private const string QuestionVotesKeyPattern = "question:{0}:votes";
    private const string UserVotesKeyPattern = "user:{0}:votes";

    public static string GetQuestionKey(long id) => string.Format(QuestionKeyPattern, id);
    public static string GetTagQuestionsKey(long tagId) => string.Format(TagQuestionsKeyPattern, tagId);
    public static string GetUserQuestionsKey(long userId) => string.Format(UserQuestionsKeyPattern, userId);

    public static string GetTagKey(long id) => string.Format(TagKeyPattern, id);
    public static string GetQuestionTagsKey(long id) => string.Format(QuestionTagsKeyPattern, id);

    public static string GetViewKey(long id) => string.Format(ViewKeyPattern, id);
    public static string GetUserViewsKey(long userId) => string.Format(UserViewsKeyPattern, userId);
    public static string GetQuestionViewsKey(long questionId) => string.Format(QuestionViewsKeyPattern, questionId);

    public static string GetVoteKey(long questionId, long userId) => string.Format(VoteKeyPattern, questionId, userId);
    public static string GetQuestionVotesKey(long questionId) => string.Format(QuestionVotesKeyPattern, questionId);
    public static string GetUserVotesKey(long userId) => string.Format(UserVotesKeyPattern, userId);


    public static long GetIdFromKey(string key)
    {
        var parts = key.Split(':');
        if (parts.Length < 2)
            throw new ArgumentException($"Invalid key format: {key}");

        return long.Parse(parts[1]);
    }
}