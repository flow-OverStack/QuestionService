using AutoMapper;
using QuestionService.Domain.Dtos.Entity;

namespace QuestionService.Grpc.Mapping;

public class RoleMapping : Profile
{
    public RoleMapping()
    {
        CreateMap<GrpcRole, RoleDto>().ReverseMap();
    }
}