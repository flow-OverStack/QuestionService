using AutoMapper;
using QuestionService.Domain.Dtos.ExternalEntity;

namespace QuestionService.GrpcClient.Mappings;

public class RoleMapping : Profile
{
    public RoleMapping()
    {
        CreateMap<GrpcRole, RoleDto>().ReverseMap();
    }
}