using AutoMapper;
using QuestionService.Domain.Dtos.GraphQl;
using QuestionService.GraphQlClient;

namespace QuestionService.Application.Mappings;

public class GraphQlUserMapping : Profile
{
    public GraphQlUserMapping()
    {
        CreateMap<IGetUserById_Users, UserDto>().ReverseMap();
    }
}