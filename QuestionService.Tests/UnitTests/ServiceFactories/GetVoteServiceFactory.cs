using QuestionService.Application.Services;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repositories;
using QuestionService.Domain.Interfaces.Services;
using QuestionService.Tests.Configurations;

namespace QuestionService.Tests.UnitTests.ServiceFactories;

public class GetVoteServiceFactory
{
    private readonly IGetVoteService _getVoteService;

    public readonly IBaseRepository<Question> QuestionRepository =
        MockRepositoriesGetters.GetMockQuestionRepository().Object;

    public readonly IBaseRepository<Vote> VoteRepository = MockRepositoriesGetters.GetMockVoteRepository().Object;

    public GetVoteServiceFactory()
    {
        _getVoteService = new GetVoteService(VoteRepository, QuestionRepository);
    }

    public IGetVoteService GetVoteService()
    {
        return _getVoteService;
    }
}