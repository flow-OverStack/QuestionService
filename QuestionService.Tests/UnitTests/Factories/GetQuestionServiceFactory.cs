using QuestionService.Application.Services;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Providers;
using QuestionService.Domain.Interfaces.Repositories;
using QuestionService.Domain.Interfaces.Services;
using QuestionService.Tests.Configurations;

namespace QuestionService.Tests.UnitTests.Factories;

internal class GetQuestionServiceFactory
{
    private readonly IGetQuestionService _getQuestionService;

    public readonly IBaseRepository<Question> QuestionRepository =
        MockRepositoriesGetters.GetMockQuestionRepository().Object;

    public readonly IBaseRepository<Tag> TagRepository = MockRepositoriesGetters.GetMockTagRepository().Object;
    public readonly IEntityProvider<UserDto> UserProvider = MockEntityProvidersGetters.GetMockUserProvider().Object;

    public GetQuestionServiceFactory()
    {
        _getQuestionService = new GetQuestionService(QuestionRepository, TagRepository);
    }

    public IGetQuestionService GetService()
    {
        return _getQuestionService;
    }
}