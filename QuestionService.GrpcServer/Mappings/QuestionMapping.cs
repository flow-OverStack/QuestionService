using AutoMapper;
using QuestionService.Domain.Entities;

namespace QuestionService.GrpcServer.Mappings;

public class QuestionMapping : Profile
{
    public QuestionMapping()
    {
        CreateMap<Question, GrpcQuestion>().ReverseMap();
    }
}