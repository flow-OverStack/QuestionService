namespace QuestionService.Tests.FunctionalTests.Helper;

internal static class GraphQlHelper
{
    public const string GraphQlEndpoint = "/graphql";

    public const string RequestAllQuery = """
                                          {
                                            questions(after: "e30x", first: 2, order: [{ id: ASC }]) {
                                              edges {
                                                cursor
                                                node {
                                                  id
                                                  title
                                                  body
                                                  reputation
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
                                                    voteType {
                                                      id
                                                      name
                                                      votes {
                                                        userId
                                                      }
                                                    }
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
                                              }
                                              pageInfo {
                                                endCursor
                                                startCursor
                                                hasNextPage
                                                hasPreviousPage
                                              }
                                              totalCount
                                            }

                                            votes(skip: 1, take: 2) {
                                              items {
                                                userId
                                                voteType {
                                                  id
                                                  name
                                                  votes {
                                                    userId
                                                  }
                                                }
                                                question {
                                                  id
                                                  title
                                                }
                                              }
                                              totalCount
                                            }

                                            tags(after: "e30x", first: 2, order: [{ id: ASC }]) {
                                              edges {
                                                cursor
                                                node {
                                                  id
                                                  name
                                                  description
                                                  questions {
                                                    id
                                                    title
                                                  }
                                                }
                                              }
                                              pageInfo {
                                                endCursor
                                                startCursor
                                                hasNextPage
                                                hasPreviousPage
                                              }
                                              totalCount
                                            }

                                            views(skip: 1, take: 2, order: [{ question: { createdAt: DESC } }]) {
                                              items {
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
                                              totalCount
                                            }

                                            voteTypes(skip: 1, take: 2, order: [{ reputationChange: ASC }]) {
                                              items {
                                                id
                                                name
                                                reputationChange
                                                votes {
                                                  userId
                                                  question {
                                                    title
                                                  }
                                                }
                                              }
                                              totalCount
                                            }
                                          }
                                          """;

    public const string RequestQuestionsWithTagsQuery = """
                                                        {
                                                          questions(after: "e30x", first: 2, order: [{ id: ASC }]) {
                                                            edges {
                                                              cursor
                                                              node {
                                                                id
                                                                title
                                                                body
                                                                userId
                                                                createdAt
                                                                lastModifiedAt
                                                                tags {
                                                                  name
                                                                }
                                                              }
                                                            }
                                                            pageInfo {
                                                              endCursor
                                                              startCursor
                                                              hasNextPage
                                                              hasPreviousPage
                                                            }
                                                            totalCount
                                                          }
                                                        }
                                                        """;

    public const string RequestWithWrongArgument = """
                                                   tag(wrongArg) {
                                                     name
                                                     description
                                                     questions {
                                                       id
                                                       title
                                                     }
                                                   }
                                                   """;

    public const string RequestWithInvalidPaginationQuery = """
                                                            {
                                                              questions(after: "notValidAfter", first: -1) {
                                                                edges {
                                                                  cursor
                                                                  node {
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
                                                                      voteType {
                                                                       id
                                                                       name
                                                                       reputationChange
                                                                      }
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
                                                                }
                                                                pageInfo {
                                                                  endCursor
                                                                  startCursor
                                                                  hasNextPage
                                                                  hasPreviousPage
                                                                }
                                                                totalCount
                                                              }
                                                              votes(skip: -1, take: 101) {
                                                                items {
                                                                  userId
                                                                  voteType {
                                                                   id
                                                                   name
                                                                   reputationChange
                                                                  }
                                                                  question {
                                                                    id
                                                                    title
                                                                  }
                                                                }
                                                                totalCount
                                                              }
                                                              tags(after: "notValidAftre", last: 2, order: []) {
                                                                edges {
                                                                  cursor
                                                                  node {
                                                                    id
                                                                    name
                                                                    description
                                                                    questions {
                                                                      id
                                                                      title
                                                                    }
                                                                  }
                                                                }
                                                                pageInfo {
                                                                  endCursor
                                                                  startCursor
                                                                  hasNextPage
                                                                  hasPreviousPage
                                                                }
                                                                totalCount
                                                              }
                                                              views(skip: -1, take: -1) {
                                                                items {
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
                                                                totalCount
                                                              }
                                                            }
                                                            """;

    public static string RequestAllByIdsQuery(long questionId, long voteQuestionId, long voteUserId,
      long tagId, long viewId, long voteTypeId)
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
                  voteType {
                    id
                    name
                    reputationChange
                    votes {
                      userId
                    }
                  }
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
                voteType {
                  id
                  name
                  reputationChange
                  votes {
                    userId
                  }
                }
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
              voteType(id: $VOTETYPEID) { 
                id
                name
                votes{
                  userId
                  question{
                    title
                  }
                }
              }
            }
            """
            .Replace("$QUESTIONID", questionId.ToString())
            .Replace("$VOTEQUESTIONID", voteQuestionId.ToString())
            .Replace("$VOTEUSERID", voteUserId.ToString())
            .Replace("$TAGID", tagId.ToString())
            .Replace("$VIEWID", viewId.ToString())
            .Replace("$VOTETYPEID", voteTypeId.ToString());
    }

    public static string RequestQuestionByIdQuery(long questionId)
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
              }  
            }
            """
            .Replace("$QUESTIONID", questionId.ToString());
    }
}