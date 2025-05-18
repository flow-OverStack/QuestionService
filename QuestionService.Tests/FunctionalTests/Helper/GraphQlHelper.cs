namespace QuestionService.Tests.FunctionalTests.Helper;

internal static class GraphQlHelper
{
    public const string GraphQlEndpoint = "/graphql";

    public const string RequestAllQuery = """
                                          {
                                            questions {
                                              id
                                              title
                                              body
                                              userId
                                              createdAt
                                              lastModifiedAt
                                              tags {
                                                id
                                                name
                                                description
                                                questions {
                                                  id
                                                  title
                                                }
                                              }
                                              votes {
                                                userId
                                                reputationChange
                                                question {
                                                  id
                                                  title
                                                }
                                              }
                                              views {
                                                id
                                                questionId
                                                userId
                                                userIp
                                                userFingerprint
                                                question {
                                                  id
                                                  title
                                                }
                                              }
                                            }
                                            votes {
                                              userId
                                              reputationChange
                                              question {
                                                id
                                                title
                                              }
                                            }
                                            tags {
                                              id
                                              name
                                              description
                                              questions {
                                                id
                                                title
                                              }
                                            }
                                            views {
                                              id
                                              questionId
                                              userId
                                              userIp
                                              userFingerprint
                                              question {
                                                id
                                                title
                                              }
                                            }
                                          }
                                          """;

    public const string RequestQuestionsWithTagsQuery = """
                                                        {
                                                          questions {
                                                            id
                                                            title
                                                            body
                                                            userId
                                                            createdAt
                                                            lastModifiedAt
                                                            tags {
                                                              id
                                                              name
                                                              description
                                                              questions {
                                                                id
                                                                title
                                                              }
                                                            }
                                                          }
                                                        }
                                                        """;

    public static string RequestAllByIdsQuery(long questionId, long voteQuestionId, long voteUserId,
        long tagId, long viewId)
    {
        return """
            {
              question(id: $QUESTIONID) {
                id
                title
                body
                userId
                createdAt
                lastModifiedAt
                tags {
                  id
                  name
                  description
                  questions {
                    id
                    title
                  }
                }
                votes {
                  userId
                  reputationChange
                  question {
                    id
                    title
                  }
                }
                views {
                  id
                  questionId
                  userId
                  userIp
                  userFingerprint
                  question {
                    id
                    title
                  }
                }
              }
              vote(userId: $VOTEUSERID, questionId: $VOTEQUESTIONID) {
                userId
                reputationChange
                question {
                  id
                  title
                }
              }
              tag(id: $TAGID) {
                name
                description
                questions {
                  id
                  title
                }
              }
              view(id: $VIEWID) {
                id
                questionId
                userId
                userIp
                userFingerprint
                question {
                  id
                  title
                }
              }
            }
            """
            .Replace("$QUESTIONID", questionId.ToString())
            .Replace("$VOTEQUESTIONID", voteQuestionId.ToString())
            .Replace("$VOTEUSERID", voteUserId.ToString())
            .Replace("$TAGID", tagId.ToString())
            .Replace("$VIEWID", viewId.ToString());
    }
}