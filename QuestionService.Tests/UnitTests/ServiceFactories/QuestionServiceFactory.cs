using AutoMapper;
using Microsoft.Extensions.Options;
using QuestionService.Domain.Dtos.GraphQl;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Providers;
using QuestionService.Domain.Interfaces.Repositories;
using QuestionService.Domain.Interfaces.Services;
using QuestionService.Domain.Settings;
using QuestionService.Tests.Configurations;
using QuestionService.Tests.UnitTests.Configurations;
using MapperConfiguration = QuestionService.Tests.Configurations.MapperConfiguration;

namespace QuestionService.Tests.UnitTests.ServiceFactories;

public class QuestionServiceFactory
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

    public readonly IMapper Mapper = MapperConfiguration.GetMapperConfiguration();
    public readonly IBaseRepository<Tag> TagRepository = MockRepositoriesGetters.GetMockTagRepository().Object;

    public readonly IUnitOfWork UnitOfWork = MockRepositoriesGetters.GetMockUnitOfWork().Object;
    public readonly IEntityProvider<UserDto> UserProvider = MockEntityProvidersGetters.GetMockUserProvider().Object;

    public QuestionServiceFactory()
    {
        _questionService = new Application.Services.QuestionService(UnitOfWork, TagRepository, UserProvider,
            new OptionsWrapper<BusinessRules>(BusinessRules), Mapper);
    }

    public IQuestionService GetService()
    {
        return _questionService;
    }
}