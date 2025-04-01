namespace QuestionService.Domain.Dtos.ExternalEntity;

public class RoleDto
{
    public long Id { get; set; }
    public string Name { get; set; }

    // Users are not implemented here because
    // GrpcRole has no filed Users to avoid cyclic dependencies
}