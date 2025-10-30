using MockQueryable.Moq;
using Moq;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Interfaces.Database;
using QuestionService.Domain.Interfaces.Repository;
using View = QuestionService.Domain.Entities.View;

namespace QuestionService.Tests.Configurations;

internal static class MockRepositoriesGetters
{
    private static IMock<ITransaction> GetMockTransaction()
    {
        return new Mock<ITransaction>();
    }

    public static IMock<IUnitOfWork> GetMockUnitOfWork()
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        mockUnitOfWork.Setup(x => x.Questions).Returns(GetMockQuestionRepository().Object);
        mockUnitOfWork.Setup(x => x.Votes).Returns(GetMockVoteRepository().Object);
        mockUnitOfWork.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetMockTransaction().Object);

        return mockUnitOfWork;
    }

    public static IMock<IBaseRepository<Question>> GetMockQuestionRepository()
    {
        var mockRepository = new Mock<IBaseRepository<Question>>();
        var questions = GetQuestions().BuildMockDbSet();

        mockRepository.Setup(x => x.GetAll()).Returns(questions.Object);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<Question>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Question question, CancellationToken _) => question);
        mockRepository.Setup(x => x.Remove(It.IsAny<Question>())).Returns((Question question) => question);
        mockRepository.Setup(x => x.Update(It.IsAny<Question>())).Returns((Question question) => question);

        return mockRepository;
    }

    public static IMock<IBaseRepository<Vote>> GetMockVoteRepository()
    {
        var mockRepository = new Mock<IBaseRepository<Vote>>();
        var votes = GetVotes().BuildMockDbSet();

        mockRepository.Setup(x => x.GetAll()).Returns(votes.Object);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<Vote>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vote vote, CancellationToken _) => vote);
        mockRepository.Setup(x => x.Remove(It.IsAny<Vote>())).Returns((Vote vote) => vote);
        mockRepository.Setup(x => x.Update(It.IsAny<Vote>())).Returns((Vote vote) => vote);

        return mockRepository;
    }

    public static IMock<IBaseRepository<VoteType>> GetMockVoteTypeRepository()
    {
        var mockRepository = new Mock<IBaseRepository<VoteType>>();
        var voteTypes = GetVoteTypes().BuildMockDbSet();

        mockRepository.Setup(x => x.GetAll()).Returns(voteTypes.Object);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<VoteType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((VoteType voteType, CancellationToken _) => voteType);
        mockRepository.Setup(x => x.Remove(It.IsAny<VoteType>())).Returns((VoteType voteType) => voteType);
        mockRepository.Setup(x => x.Update(It.IsAny<VoteType>())).Returns((VoteType voteType) => voteType);

        return mockRepository;
    }

    public static IMock<IBaseRepository<Tag>> GetMockTagRepository()
    {
        var mockRepository = new Mock<IBaseRepository<Tag>>();

        // Adding questions to tags

        var tags = GetTags().ToList();
        tags.ForEach(x => x.Questions = []);
        var questions = GetQuestions().ToList();
        var questionTags = GetQuestionTags().ToList();

        foreach (var questionTag in questionTags)
        {
            var tag = tags.First(x => x.Id == questionTag.TagId);
            var question = questions.First(x => x.Id == questionTag.QuestionId);

            tag.Questions.Add(question);
        }


        var tagsDbSet = tags.BuildMockDbSet();

        mockRepository.Setup(x => x.GetAll()).Returns(tagsDbSet.Object);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<Tag>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tag tag, CancellationToken _) => tag);
        mockRepository.Setup(x => x.Remove(It.IsAny<Tag>())).Returns((Tag tag) => tag);
        mockRepository.Setup(x => x.Update(It.IsAny<Tag>())).Returns((Tag tag) => tag);

        return mockRepository;
    }

    public static IMock<IBaseRepository<View>> GetMockViewRepository()
    {
        var mockRepository = new Mock<IBaseRepository<View>>();

        var views = GetViews().BuildMockDbSet();

        mockRepository.Setup(x => x.GetAll()).Returns(views.Object);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<View>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((View view, CancellationToken _) => view);
        mockRepository.Setup(x => x.Remove(It.IsAny<View>())).Returns((View view) => view);
        mockRepository.Setup(x => x.Update(It.IsAny<View>())).Returns((View view) => view);

        return mockRepository;
    }

    public static IQueryable<Question> GetQuestions()
    {
        return new Question[]
        {
            new()
            {
                Id = 1,
                Title = "question1",
                Body = "questionBody1",
                UserId = 1,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow.AddSeconds(Random.Shared.Next(1, 20)),
                Tags = [GetTagDotNet()],
                Votes = [],
                Views = GetViews().Where(x => x.QuestionId == 1).ToList()
            },
            new()
            {
                Id = 2,
                Title = "question2",
                Body = "questionBody2",
                UserId = 1,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow.AddSeconds(Random.Shared.Next(1, 20)),
                Tags = [GetTagDotNet(), GetTagJava()],
                Votes = GetVotes().Where(x => x.QuestionId == 2).ToList(),
                Views = GetViews().Where(x => x.QuestionId == 2).ToList()
            },
            new()
            {
                Id = 3,
                Title = "question3",
                Body = "questionBody3",
                UserId = 3,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow.AddSeconds(Random.Shared.Next(1, 20)),
                Tags = [GetTagDotNet(), GetTagJava(), GetTagPython()],
                Votes = GetVotes().Where(x => x.QuestionId == 3).ToList(),
                Views = GetViews().Where(x => x.QuestionId == 3).ToList()
            },
            new() // Question without tags (not possible)
            {
                Id = 4,
                Title = "question4",
                Body = "questionBody4",
                UserId = 3,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow.AddSeconds(Random.Shared.Next(1, 20)),
                Tags = [],
                Votes = [],
                Views = GetViews().Where(x => x.QuestionId == 4).ToList()
            }
        }.AsQueryable();
    }

    public static IQueryable<Vote> GetVotes()
    {
        return new[]
        {
            GetUpvote(1, 2), GetUpvote(2, 2), GetDownvote(4, 2),
            GetUpvote(2, 3), GetDownvote(1, 3), GetDownvote(4, 3)
        }.AsQueryable();
    }

    public static IQueryable<VoteType> GetVoteTypes()
    {
        return new[]
        {
            new VoteType
            {
                Id = 1,
                Name = nameof(VoteTypes.Upvote)
            },
            new VoteType
            {
                Id = 2,
                Name = nameof(VoteTypes.Downvote)
            }
        }.AsQueryable();
    }

    public static IQueryable<Tag> GetTags()
    {
        return new[]
        {
            GetTagDotNet(),
            GetTagJava(),
            GetTagPython()
        }.AsQueryable();
    }

    public static IQueryable<QuestionTag> GetQuestionTags()
    {
        return new[]
        {
            new QuestionTag
            {
                QuestionId = 1,
                TagId = 1
            },
            new QuestionTag
            {
                QuestionId = 2,
                TagId = 1
            },
            new QuestionTag
            {
                QuestionId = 2,
                TagId = 2
            },
            new QuestionTag
            {
                QuestionId = 3,
                TagId = 1
            },
            new QuestionTag
            {
                QuestionId = 3,
                TagId = 2
            },
            new QuestionTag
            {
                QuestionId = 3,
                TagId = 3
            }
        }.AsQueryable();
    }

    public static IQueryable<View> GetViews()
    {
        return new[]
        {
            new View
            {
                Id = 1,
                QuestionId = 1,
                UserId = 1,
                UserIp = null,
                UserFingerprint = null
            },
            new View
            {
                Id = 2,
                QuestionId = 2,
                UserId = 1,
                UserIp = null,
                UserFingerprint = null
            },
            new View
            {
                Id = 3,
                QuestionId = 2,
                UserId = null,
                UserIp = "0.0.0.0",
                UserFingerprint = "testFingerprint3"
            },
            new View
            {
                Id = 4,
                QuestionId = 3,
                UserId = null,
                UserIp = "1.0.0.1",
                UserFingerprint = "testFingerprint4"
            },
        }.AsQueryable();
    }

    // Get entities methods

    private static Tag GetTagDotNet()
    {
        return new Tag
        {
            Id = 1,
            Name = ".NET",
            Description = ".NET is a secure, reliable, and high-performance application platform."
        };
    }

    private static Tag GetTagJava()
    {
        return new Tag
        {
            Id = 2,
            Name = "Java",
            Description = "Java is a high-level, general-purpose, memory-safe, object-oriented programming language."
        };
    }

    private static Tag GetTagPython()
    {
        return new Tag
        {
            Id = 3,
            Name = "Python",
            Description = "Python is a high-level, general-purpose programming language."
        };
    }

    private static Vote GetUpvote(long userId, long questionId)
    {
        return new Vote
        {
            UserId = userId,
            QuestionId = questionId,
            VoteType = new VoteType { Id = 1, Name = nameof(VoteTypes.Upvote) }
        };
    }

    private static Vote GetDownvote(long userId, long questionId)
    {
        return new Vote
        {
            UserId = userId,
            QuestionId = questionId,
            VoteType = new VoteType { Id = 2, Name = nameof(VoteTypes.Downvote) }
        };
    }
}