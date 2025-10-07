using QuestionService.Application.Services;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Tests.Configurations;

namespace QuestionService.Tests.UnitTests.Factories;

internal class GetTagServiceFactory
{
    private readonly IGetTagService _getTagService;

    public readonly IBaseRepository<Question> QuestionRepository =
        MockRepositoriesGetters.GetMockQuestionRepository().Object;

    public readonly IBaseRepository<Tag> TagRepository = MockRepositoriesGetters.GetMockTagRepository().Object;


    public GetTagServiceFactory()
    {
        _getTagService = new GetTagService(TagRepository, QuestionRepository);
    }

    public IGetTagService GetService()
    {
        return _getTagService;
    }
}