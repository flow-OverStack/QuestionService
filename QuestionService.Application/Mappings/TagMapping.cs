using AutoMapper;
using QuestionService.Domain.Dtos.Tag;
using QuestionService.Domain.Entities;

namespace QuestionService.Application.Mappings;

public class TagMapping : Profile
{
    public TagMapping()
    {
        CreateMap<Tag, TagDto>().ReverseMap();
    }
}