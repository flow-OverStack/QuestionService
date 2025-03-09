using AutoMapper;
using QuestionService.Domain.Dtos.GraphQl;

namespace QuestionService.Grpc.Mapping;

public class RoleMapping : Profile
{
    public RoleMapping()
    {
        CreateMap<GrpcRole, RoleDto>().ReverseMap();
    }
}