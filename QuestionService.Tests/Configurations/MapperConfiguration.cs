using AutoMapper;
using QuestionService.Application.Mappings;

namespace QuestionService.Tests.Configurations;

internal static class MapperConfiguration
{
    public static IMapper GetMapperConfiguration()
    {
        var mockMapper = new AutoMapper.MapperConfiguration(cfg => cfg.AddMaps(typeof(QuestionMapping)));
        return mockMapper.CreateMapper();
    }
}