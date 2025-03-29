using Microsoft.EntityFrameworkCore.Storage;
using MockQueryable.Moq;
using Moq;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repositories;

namespace QuestionService.Tests.Configurations;

internal static class MockRepositoriesGetters
{
    private static Mock<IDbContextTransaction> GetMockTransaction()
    {
        var transaction = new Mock<IDbContextTransaction>();

        transaction.Setup(x => x.CommitAsync(default)).Returns(Task.CompletedTask);
        transaction.Setup(x => x.RollbackAsync(default)).Returns(Task.CompletedTask);

        return transaction;
    }

    public static Mock<IUnitOfWork> GetMockUnitOfWork()
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        mockUnitOfWork.Setup(x => x.Questions).Returns(GetMockQuestionRepository().Object);
        mockUnitOfWork.Setup(x => x.Votes).Returns(GetMockVoteRepository().Object);
        mockUnitOfWork.Setup(x => x.BeginTransactionAsync()).ReturnsAsync(GetMockTransaction().Object);
        mockUnitOfWork.Setup(x => x.SaveChangesAsync());

        return mockUnitOfWork;
    }

    public static Mock<IBaseRepository<Question>> GetMockQuestionRepository()
    {
        var mockRepository = new Mock<IBaseRepository<Question>>();
        var questions = GetQuestions().BuildMockDbSet();

        mockRepository.Setup(x => x.GetAll()).Returns(questions.Object);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<Question>())).ReturnsAsync((Question question) => question);
        mockRepository.Setup(x => x.Remove(It.IsAny<Question>())).Returns((Question question) => question);
        mockRepository.Setup(x => x.Update(It.IsAny<Question>())).Returns((Question question) => question);

        return mockRepository;
    }

    public static Mock<IBaseRepository<Vote>> GetMockVoteRepository()
    {
        var mockRepository = new Mock<IBaseRepository<Vote>>();
        var votes = GetVotes().BuildMockDbSet();

        mockRepository.Setup(x => x.GetAll()).Returns(votes.Object);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<Vote>())).ReturnsAsync((Vote vote) => vote);
        mockRepository.Setup(x => x.Remove(It.IsAny<Vote>())).Returns((Vote vote) => vote);
        mockRepository.Setup(x => x.Update(It.IsAny<Vote>())).Returns((Vote vote) => vote);

        return mockRepository;
    }

    public static Mock<IBaseRepository<Tag>> GetMockTagRepository()
    {
        var mockRepository = new Mock<IBaseRepository<Tag>>();

        #region Adding questions to tags

        var tags = GetTags().ToList();
        tags.ForEach(x => x.Questions = []);
        var questions = GetQuestions().ToList();
        var questionTags = GetQuestionTags().ToList();

        foreach (var questionTag in questionTags)
        {
            var tag = tags.First(x => x.Name == questionTag.TagName);
            var question = questions.First(x => x.Id == questionTag.QuestionId);

            tag.Questions.Add(question);
        }

        #endregion

        var tagsDbSet = tags.BuildMockDbSet();

        mockRepository.Setup(x => x.GetAll()).Returns(tagsDbSet.Object);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<Tag>())).ReturnsAsync((Tag tag) => tag);
        mockRepository.Setup(x => x.Remove(It.IsAny<Tag>())).Returns((Tag tag) => tag);
        mockRepository.Setup(x => x.Update(It.IsAny<Tag>())).Returns((Tag tag) => tag);

        return mockRepository;
    }

    public static Mock<IBaseRepository<T>> GetEmptyMockRepository<T>() where T : class
    {
        var mockRepository = new Mock<IBaseRepository<T>>();
        var roles = Array.Empty<T>().BuildMockDbSet();

        mockRepository.Setup(x => x.GetAll()).Returns(roles.Object);
        mockRepository.Setup(x => x.CreateAsync(It.IsAny<T>())).ReturnsAsync((T role) => role);
        mockRepository.Setup(x => x.Update(It.IsAny<T>())).Returns((T role) => role);
        mockRepository.Setup(x => x.Remove(It.IsAny<T>())).Returns((T role) => role);

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
                Reputation = 0,
                UserId = 1,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = null,
                Tags = [GetTagDotNet()],
                Views = 0,
                Votes = []
            },
            new()
            {
                Id = 2,
                Title = "question2",
                Body = "questionBody2",
                Reputation = 2,
                UserId = 1,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow.AddMilliseconds(Random.Shared.Next(1, 20)),
                Tags = [GetTagDotNet(), GetTagJava()],
                Views = 10,
                Votes = GetVotes().Where(x => x.QuestionId == 2).ToList()
            },
            new()
            {
                Id = 3,
                Title = "question3",
                Body = "questionBody3",
                Reputation = -1,
                UserId = 3,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow.AddMilliseconds(Random.Shared.Next(1, 20)),
                Tags = [GetTagDotNet(), GetTagJava(), GetTagPython()],
                Views = 50,
                Votes = GetVotes().Where(x => x.QuestionId == 3).ToList()
            },
            new() // Question without tags
            {
                Id = 4,
                Title = "question4",
                Body = "questionBody4",
                Reputation = 0,
                UserId = 3,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow.AddMilliseconds(Random.Shared.Next(1, 20)),
                Tags = [],
                Views = 50,
                Votes = []
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
                TagName = ".NET"
            },
            new QuestionTag
            {
                QuestionId = 2,
                TagName = ".NET"
            },
            new QuestionTag
            {
                QuestionId = 2,
                TagName = "Java"
            },
            new QuestionTag
            {
                QuestionId = 3,
                TagName = ".NET"
            },
            new QuestionTag
            {
                QuestionId = 3,
                TagName = "Java"
            },
            new QuestionTag
            {
                QuestionId = 3,
                TagName = "Python"
            }
        }.AsQueryable();
    }

    #region Get entities methods

    private static Tag GetTagDotNet()
    {
        return new Tag
        {
            Name = ".NET",
            Description = ".NET is a secure, reliable, and high-performance application platform."
        };
    }

    private static Tag GetTagJava()
    {
        return new Tag
        {
            Name = "Java",
            Description = "Java is a high-level, general-purpose, memory-safe, object-oriented programming language."
        };
    }

    private static Tag GetTagPython()
    {
        return new Tag
        {
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
            ReputationChange = 1
        };
    }

    private static Vote GetDownvote(long userId, long questionId)
    {
        return new Vote
        {
            UserId = userId,
            QuestionId = questionId,
            ReputationChange = -1
        };
    }

    #endregion
}