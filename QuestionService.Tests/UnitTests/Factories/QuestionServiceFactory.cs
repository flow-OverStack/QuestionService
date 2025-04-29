using AutoMapper;
using Microsoft.Extensions.Options;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Producer;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Settings;
using QuestionService.Tests.Configurations;
using QuestionService.Tests.UnitTests.Configurations;
using MapperConfiguration = QuestionService.Tests.UnitTests.Configurations.MapperConfiguration;

namespace QuestionService.Tests.UnitTests.Factories;

internal class QuestionServiceFactory
{
    private readonly IQuestionService _questionService;

    public readonly BusinessRules BusinessRules = new()
    {
        TitleMinLength = 10,
        BodyMinLength = 30,
        MinReputationToUpvote = 15,
        MinReputationToDownvote = 125,
        DownvoteReputationChange = -1,
        UpvoteReputationChange = 1
    };

    public readonly IBaseEventProducer EventProducer =
        BaseEventProducerConfiguration.GetBaseEventProducerConfiguration();

    public readonly IMapper Mapper = MapperConfiguration.GetMapperConfiguration();
    public readonly IBaseRepository<Tag> TagRepository = MockRepositoriesGetters.GetMockTagRepository().Object;
    public readonly IUnitOfWork UnitOfWork = MockRepositoriesGetters.GetMockUnitOfWork().Object;
    public readonly IEntityProvider<UserDto> UserProvider = MockEntityProvidersGetters.GetMockUserProvider().Object;

    public QuestionServiceFactory()
    {
        _questionService = new Application.Services.QuestionService(UnitOfWork, TagRepository, UserProvider,
            new OptionsWrapper<BusinessRules>(BusinessRules), Mapper, EventProducer);
    }

    public IQuestionService GetService()
    {
        return _questionService;
    }
}