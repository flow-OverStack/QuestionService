using QuestionService.Application.Services;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Tests.Configurations;
using View = QuestionService.Domain.Entities.View;

namespace QuestionService.Tests.UnitTests.Factories;

internal class GetViewServiceFactory
{
    private readonly IGetViewService _getViewService;

    public readonly IBaseRepository<Question> QuestionRepository =
        MockRepositoriesGetters.GetMockQuestionRepository().Object;

    public readonly IBaseRepository<View> ViewRepository = MockRepositoriesGetters.GetMockViewRepository().Object;

    public GetViewServiceFactory()
    {
        _getViewService = new GetViewService(ViewRepository);
    }

    public IGetViewService GetService()
    {
        return _getViewService;
    }
}