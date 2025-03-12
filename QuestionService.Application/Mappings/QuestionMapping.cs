using AutoMapper;
using QuestionService.Domain.Dtos.Question;
using QuestionService.Domain.Entities;

namespace QuestionService.Application.Mappings;

public class QuestionMapping : Profile
{
    public QuestionMapping()
    {
        CreateMap<Question, QuestionDto>()
            .ForCtorParam("TagNames", opt => opt.MapFrom(x => x.Tags.Select(y => y.Name).ToArray()))
            .ReverseMap()
            .ForMember(x => x.Tags, opt => opt.MapFrom(x => x.TagNames.Select(y => new Tag { Name = y }).ToList()));
        CreateMap<Question, AskQuestionDto>()
            .ForCtorParam("TagNames", opt => opt.MapFrom(x => x.Tags.Select(y => y.Name).ToArray()))
            .ReverseMap()
            .ForMember(x => x.Tags, opt => opt.MapFrom(x => x.TagNames.Select(y => new Tag { Name = y }).ToList()));
        CreateMap<Question, EditQuestionDto>()
            .ForCtorParam("TagNames", opt => opt.MapFrom(x => x.Tags.Select(y => y.Name).ToArray()))
            .ReverseMap()
            .ForMember(x => x.Tags, opt => opt.MapFrom(x => x.TagNames.Select(y => new Tag { Name = y }).ToList()))
            .ForMember(x => x.Id, opt => opt.Ignore());
        CreateMap<Question, VoteQuestionDto>().ReverseMap();
    }
}