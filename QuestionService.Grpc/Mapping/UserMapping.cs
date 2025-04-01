using AutoMapper;
using QuestionService.Domain.Dtos.ExternalEntity;

namespace QuestionService.Grpc.Mapping;

public class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<GrpcUser, UserDto>().ReverseMap();
    }
}