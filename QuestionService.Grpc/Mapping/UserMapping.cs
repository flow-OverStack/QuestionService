using AutoMapper;
using QuestionService.Domain.Dtos.Entity;

namespace QuestionService.Grpc.Mapping;

public class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<GrpcUser, UserDto>().ReverseMap();
    }
}