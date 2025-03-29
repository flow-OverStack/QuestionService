using QuestionService.Application.Services;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repositories;
using QuestionService.Domain.Interfaces.Services;
using QuestionService.Tests.Configurations;

namespace QuestionService.Tests.UnitTests.ServiceFactories;

public class GetTagServiceFactory
{
    private readonly IGetTagService _getTagService;

    public readonly IBaseRepository<Question> QuestionRepository =
        MockRepositoriesGetters.GetMockQuestionRepository().Object;

    public readonly IBaseRepository<Tag> TagRepository = MockRepositoriesGetters.GetMockTagRepository().Object;


    public GetTagServiceFactory(IBaseRepository<Tag>? tagRepository = null)
    {
        if (tagRepository != null) TagRepository = tagRepository;

        _getTagService = new GetTagService(TagRepository, QuestionRepository);
    }

    public IGetTagService GetService()
    {
        return _getTagService;
    }
}