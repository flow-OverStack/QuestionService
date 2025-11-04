using QuestionService.Application.Services;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Tests.Configurations;

namespace QuestionService.Tests.UnitTests.Factories;

public class GetVoteTypeServiceFactory
{
    private readonly IGetVoteTypeService _getVoteTypeService;

    public IBaseRepository<VoteType> VoteTypeRepository = MockRepositoriesGetters.GetMockVoteTypeRepository().Object;

    public GetVoteTypeServiceFactory()
    {
        _getVoteTypeService = new GetVoteTypeService(VoteTypeRepository);
    }

    public IGetVoteTypeService GetService()
    {
        return _getVoteTypeService;
    }
}