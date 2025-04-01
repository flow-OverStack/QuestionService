namespace QuestionService.Domain.Dtos.ExternalEntity;

public class UserDto
{
    public long Id { get; set; }
    public Guid KeycloakId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public DateTime LastLoginAt { get; set; }
    public int Reputation { get; set; }
    public IEnumerable<RoleDto> Roles { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ReputationEarnedToday { get; set; }
}