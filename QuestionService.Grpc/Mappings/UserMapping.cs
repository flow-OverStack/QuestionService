using AutoMapper;
using QuestionService.Domain.Dtos.ExternalEntity;

namespace QuestionService.Grpc.Mappings;

public class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<GrpcUser, UserDto>().ReverseMap();
    }
}