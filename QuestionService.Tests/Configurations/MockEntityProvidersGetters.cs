using Moq;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Interfaces.Provider;

namespace QuestionService.Tests.Configurations;

internal static class MockEntityProvidersGetters
{
    public const int MinReputation = 1;

    public static IMock<IEntityProvider<UserDto>> GetMockUserProvider()
    {
        var mockProvider = new Mock<IEntityProvider<UserDto>>();

        mockProvider.Setup(x => x.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((long userId, CancellationToken _) => GetUserDtos().FirstOrDefault(x => x.Id == userId));
        mockProvider.Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<long>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IEnumerable<long> userIds, CancellationToken _) =>
                GetUserDtos().Where(x => userIds.Contains(x.Id)));

        return mockProvider;
    }

    public static IQueryable<UserDto> GetUserDtos()
    {
        return new UserDto[]
        {
            new()
            {
                Id = 1,
                IdentityId = Guid.NewGuid(),
                Username = "testuser1",
                Email = "TestUser1@test.com",
                LastLoginAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Reputation = 125,
                Roles = [GetRoleUser(), GetRoleAdmin()]
            },
            new()
            {
                Id = 2,
                IdentityId = Guid.NewGuid(),
                Username = "testuser2",
                Email = "TestUser2@test.com",
                LastLoginAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Reputation = 15,
                Roles = [GetRoleUser()]
            },
            new()
            {
                Id = 3,
                IdentityId = Guid.NewGuid(),
                Username = "testuser3",
                Email = "TestUser3@test.com",
                LastLoginAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Reputation = MinReputation,
                Roles = [GetRoleModer()]
            },
            new()
            {
                Id = 4,
                IdentityId = Guid.NewGuid(),
                Username = "testuser4",
                Email = "TestUser4@test.com",
                LastLoginAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Reputation = 125,
                Roles = [GetRoleModer()]
            }
        }.AsQueryable();
    }

    // Get entity dtos methods

    private static RoleDto GetRoleUser()
    {
        return new RoleDto
        {
            Id = 1,
            Name = "User"
        };
    }

    private static RoleDto GetRoleAdmin()
    {
        return new RoleDto
        {
            Id = 2,
            Name = "Admin"
        };
    }

    private static RoleDto GetRoleModer()
    {
        return new RoleDto
        {
            Id = 3,
            Name = "Moderator"
        };
    }
}