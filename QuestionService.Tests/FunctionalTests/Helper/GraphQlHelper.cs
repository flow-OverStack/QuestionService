namespace QuestionService.Tests.FunctionalTests.Helper;

public static class GraphQlHelper
{
    public const string GraphQlEndpoint = "/graphql";

    public const string RequestAllQuery = """
                                          {
                                            questions {
                                              id
                                              title
                                              body
                                              views
                                              userId
                                              reputation
                                              createdAt
                                              lastModifiedAt
                                              tags {
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
                                              name
                                              description
                                              questions {
                                                id
                                                title
                                              }
                                            }
                                          }
                                          """;

    public const string RequestTagsQuery = """
                                           {
                                             tags {
                                               name
                                               description
                                               questions {
                                                 id
                                                 title
                                               }
                                             }
                                           } 
                                           """;

    public static string RequestAllByIdsAndNamesQuery(long questionId, long voteQuestionId, long voteUserId,
        string tagName)
    {
        return """
            {
              question(id: $QUESTIONID) {
                id
                title
                body
                views
                userId
                reputation
                createdAt
                lastModifiedAt
                tags {
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
              }
              vote(questionId: $VOTEQUESTIONID, userId: $VOTEUSERID){
                reputationChange
                question{
                  id
                title
                }
              }
              tag(name: "$TAGNAME"){
                description
                questions{
                  id
                  title
                }
              }
            }
            """
            .Replace("$QUESTIONID", questionId.ToString())
            .Replace("$VOTEQUESTIONID", voteQuestionId.ToString())
            .Replace("$VOTEUSERID", voteUserId.ToString())
            .Replace("$TAGNAME", tagName);
    }
}