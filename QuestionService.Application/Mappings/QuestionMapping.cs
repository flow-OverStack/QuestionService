using AutoMapper;
using QuestionService.Domain.Dtos.Question;
using QuestionService.Domain.Entities;

namespace QuestionService.Application.Mappings;

public class QuestionMapping : Profile
{
    public QuestionMapping()
    {
        CreateMap<Question, QuestionDto>().ReverseMap();
        CreateMap<Question, AskQuestionDto>().ReverseMap();
        CreateMap<Question, EditQuestionDto>().ReverseMap();
    }
}