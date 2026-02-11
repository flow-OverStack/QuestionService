using AutoMapper;
using QuestionService.Application.Validators;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Producer;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Tests.Configurations;
using QuestionService.Tests.UnitTests.Configurations;
using MapperConfiguration = QuestionService.Tests.UnitTests.Configurations.MapperConfiguration;

namespace QuestionService.Tests.UnitTests.Factories;

internal class QuestionServiceFactory
{
    private readonly IQuestionService _questionService;

    public readonly IBaseEventProducer EventProducer =
        BaseEventProducerConfiguration.GetBaseEventProducerConfiguration();

    public readonly IMapper Mapper = MapperConfiguration.GetMapperConfiguration();
    public readonly IBaseRepository<Tag> TagRepository = MockRepositoriesGetters.GetMockTagRepository().Object;

    public readonly IUnitOfWork UnitOfWork = MockRepositoriesGetters.GetMockUnitOfWork().Object;
    public readonly IEntityProvider<UserDto> UserProvider = MockEntityProvidersGetters.GetMockUserProvider().Object;

    public readonly IBaseRepository<VoteType> VoteTypeRepository =
        MockRepositoriesGetters.GetMockVoteTypeRepository().Object;

    public QuestionServiceFactory(IBaseRepository<VoteType>? voteTypeRepository = null)
    {
        if (voteTypeRepository != null)
            VoteTypeRepository = voteTypeRepository;

        _questionService = new Application.Services.QuestionService(UnitOfWork, TagRepository, VoteTypeRepository,
            UserProvider, Mapper, EventProducer, new QuestionValidator());
    }

    public IQuestionService GetService()
    {
        return _questionService;
    }
}