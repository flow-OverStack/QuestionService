using QuestionService.Application.Services;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Tests.Configurations;

namespace QuestionService.Tests.UnitTests.Factories;

internal class GetQuestionServiceFactory
{
    private readonly IGetQuestionService _getQuestionService;

    public readonly IBaseRepository<Question> QuestionRepository =
        MockRepositoriesGetters.GetMockQuestionRepository().Object;

    public readonly IBaseRepository<Tag> TagRepository = MockRepositoriesGetters.GetMockTagRepository().Object;

    public GetQuestionServiceFactory()
    {
        _getQuestionService = new GetQuestionService(QuestionRepository, TagRepository);
    }

    public IGetQuestionService GetService()
    {
        return _getQuestionService;
    }
}