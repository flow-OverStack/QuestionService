using AutoMapper;
using QuestionService.Application.Services;
using QuestionService.Application.Validators;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Tests.Configurations;
using MapperConfiguration = QuestionService.Tests.UnitTests.Configurations.MapperConfiguration;

namespace QuestionService.Tests.UnitTests.Factories;

internal class TagServiceFactory
{
    private readonly ITagService _tagService;

    public readonly IMapper Mapper = MapperConfiguration.GetMapperConfiguration();
    public readonly IBaseRepository<Tag> TagRepository = MockRepositoriesGetters.GetMockTagRepository().Object;


    public TagServiceFactory()
    {
        _tagService = new TagService(TagRepository, Mapper, new TagValidator());
    }

    public ITagService GetService()
    {
        return _tagService;
    }
}