using QuestionService.Application.Services;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repositories;
using QuestionService.Domain.Interfaces.Services;
using QuestionService.Tests.Configurations;
using View = QuestionService.Domain.Entities.View;

namespace QuestionService.Tests.UnitTests.Factories;

public class GetViewServiceFactory
{
    private readonly IGetViewService _getViewService;

    public readonly IBaseRepository<Question> QuestionRepository =
        MockRepositoriesGetters.GetMockQuestionRepository().Object;

    public readonly IBaseRepository<View> ViewRepository = MockRepositoriesGetters.GetMockViewRepository().Object;

    public GetViewServiceFactory()
    {
        _getViewService = new GetViewService(ViewRepository, QuestionRepository);
    }

    public IGetViewService GetService()
    {
        return _getViewService;
    }
}