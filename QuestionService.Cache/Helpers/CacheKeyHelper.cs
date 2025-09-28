namespace QuestionService.Cache.Helpers;

public static class CacheKeyHelper
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

    private const string ViewQuestionKeyPattern = "view:question:{0}";
    private const string ViewQuestionsKey = "view:questions";

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

    public static string GetViewQuestionsKey() => ViewQuestionsKey;
    public static string GetViewQuestionKey(long questionId) => string.Format(ViewQuestionKeyPattern, questionId);


    public static long GetIdFromKey(string key)
    {
        var parts = key.Split(':');

        var ex = new ArgumentException($"Invalid key format: {key}");
        return parts.Length switch
        {
            2 => long.Parse(parts[1]),
            3 => TryParseLong(parts[1]) ??
                 TryParseLong(parts[2]) ?? throw ex,
            _ => throw ex
        };
    }

    private static long? TryParseLong(string str) => long.TryParse(str, out var result) ? result : null;
}