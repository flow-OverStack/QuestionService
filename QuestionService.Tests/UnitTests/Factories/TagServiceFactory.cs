using AutoMapper;
using Microsoft.Extensions.Options;
using QuestionService.Application.Services;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Settings;
using QuestionService.Tests.Configurations;
using QuestionService.Tests.UnitTests.Configurations;
using MapperConfiguration = QuestionService.Tests.UnitTests.Configurations.MapperConfiguration;

namespace QuestionService.Tests.UnitTests.Factories;

internal class TagServiceFactory
{
    private readonly ITagService _tagService;

    public readonly BusinessRules BusinessRules = BusinessRulesConfiguration.GetBusinessRules();
    public readonly IMapper Mapper = MapperConfiguration.GetMapperConfiguration();
    public readonly IBaseRepository<Tag> TagRepository = MockRepositoriesGetters.GetMockTagRepository().Object;


    public TagServiceFactory()
    {
        _tagService = new TagService(TagRepository, Mapper, new OptionsWrapper<BusinessRules>(BusinessRules));
    }

    public ITagService GetService()
    {
        return _tagService;
    }
}