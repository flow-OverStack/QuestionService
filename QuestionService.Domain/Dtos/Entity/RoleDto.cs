namespace QuestionService.Domain.Dtos.Entity;

public class RoleDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<UserDto> Users { get; set; }
}