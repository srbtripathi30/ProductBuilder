using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using ProductBuilder.Domain.Entities;
using ProductBuilder.Infrastructure.Services;

namespace ProductBuilder.Tests.Services;

public class TokenServiceTests
{
    private static TokenService CreateService()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:SecretKey", "test-secret-key-must-be-at-least-32-chars!!" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" },
                { "Jwt:AccessTokenExpiryMinutes", "60" }
            })
            .Build();
        return new TokenService(config);
    }

    private static User TestUser() => new()
    {
        Id = Guid.NewGuid(),
        Email = "test@example.com",
        FirstName = "John",
        LastName = "Doe",
        PasswordHash = "hash",
        RoleId = 1,
        Role = new Role { Id = 1, Name = "Admin" },
        IsActive = true,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    [Fact]
    public void GenerateAccessToken_ReturnsNonEmptyString()
    {
        var token = CreateService().GenerateAccessToken(TestUser());
        Assert.NotEmpty(token);
    }

    [Fact]
    public void GenerateAccessToken_IsValidJwt()
    {
        var token = CreateService().GenerateAccessToken(TestUser());
        Assert.True(new JwtSecurityTokenHandler().CanReadToken(token));
    }

    [Fact]
    public void GenerateAccessToken_ContainsCorrectEmail()
    {
        var user = TestUser();
        var token = CreateService().GenerateAccessToken(user);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var emailClaim = jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value;
        Assert.Equal(user.Email, emailClaim);
    }

    [Fact]
    public void GenerateAccessToken_ContainsCorrectRole()
    {
        var user = TestUser();
        var token = CreateService().GenerateAccessToken(user);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Contains(jwt.Claims, c => c.Value == "Admin");
    }

    [Fact]
    public void GetUserIdFromToken_ReturnsCorrectUserId()
    {
        var svc = CreateService();
        var user = TestUser();
        var token = svc.GenerateAccessToken(user);
        var extractedId = svc.GetUserIdFromToken(token);
        Assert.Equal(user.Id, extractedId);
    }

    [Fact]
    public void GetUserIdFromToken_InvalidToken_ReturnsNull()
    {
        var result = CreateService().GetUserIdFromToken("not-a-valid-token");
        Assert.Null(result);
    }

    [Fact]
    public void GetUserIdFromToken_EmptyString_ReturnsNull()
    {
        var result = CreateService().GetUserIdFromToken("");
        Assert.Null(result);
    }

    [Fact]
    public void GenerateRefreshToken_ReturnsNonEmptyString()
    {
        var token = CreateService().GenerateRefreshToken();
        Assert.NotEmpty(token);
    }

    [Fact]
    public void GenerateRefreshToken_IsBase64()
    {
        var token = CreateService().GenerateRefreshToken();
        var bytes = Convert.FromBase64String(token);
        Assert.Equal(64, bytes.Length); // 64 random bytes
    }

    [Fact]
    public void GenerateRefreshToken_ReturnsDifferentValuesEachTime()
    {
        var svc = CreateService();
        Assert.NotEqual(svc.GenerateRefreshToken(), svc.GenerateRefreshToken());
    }

    [Fact]
    public void GenerateAccessToken_DifferentUsersProduceDifferentTokens()
    {
        var svc = CreateService();
        var u1 = TestUser();
        var u2 = TestUser();
        u2.Id = Guid.NewGuid();
        u2.Email = "other@example.com";
        Assert.NotEqual(svc.GenerateAccessToken(u1), svc.GenerateAccessToken(u2));
    }
}
