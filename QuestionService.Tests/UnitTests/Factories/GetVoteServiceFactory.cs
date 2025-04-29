using QuestionService.Application.Services;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Tests.Configurations;

namespace QuestionService.Tests.UnitTests.Factories;

internal class GetVoteServiceFactory
{
    private readonly IGetVoteService _getVoteService;

    public readonly IBaseRepository<Question> QuestionRepository =
        MockRepositoriesGetters.GetMockQuestionRepository().Object;

    public readonly IBaseRepository<Vote> VoteRepository = MockRepositoriesGetters.GetMockVoteRepository().Object;

    public GetVoteServiceFactory()
    {
        _getVoteService = new GetVoteService(VoteRepository, QuestionRepository);
    }

    public IGetVoteService GetService()
    {
        return _getVoteService;
    }
}