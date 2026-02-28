using ProductBuilder.Infrastructure.Services;

namespace ProductBuilder.Tests.Services;

public class PasswordServiceTests
{
    private static PasswordService Svc() => new();

    [Fact]
    public void HashPassword_ReturnsNonEmptyString()
    {
        var hash = Svc().HashPassword("MyPassword123!");
        Assert.NotEmpty(hash);
    }

    [Fact]
    public void HashPassword_ProducesDifferentHashesForSamePassword()
    {
        // BCrypt uses a random salt each time
        var svc = Svc();
        var h1 = svc.HashPassword("MyPassword123!");
        var h2 = svc.HashPassword("MyPassword123!");
        Assert.NotEqual(h1, h2);
    }

    [Fact]
    public void VerifyPassword_CorrectPassword_ReturnsTrue()
    {
        var svc = Svc();
        var hash = svc.HashPassword("correct-password");
        Assert.True(svc.VerifyPassword("correct-password", hash));
    }

    [Fact]
    public void VerifyPassword_WrongPassword_ReturnsFalse()
    {
        var svc = Svc();
        var hash = svc.HashPassword("correct-password");
        Assert.False(svc.VerifyPassword("wrong-password", hash));
    }

    [Fact]
    public void VerifyPassword_EmptyPassword_ReturnsFalse()
    {
        var svc = Svc();
        var hash = svc.HashPassword("some-password");
        Assert.False(svc.VerifyPassword("", hash));
    }

    [Fact]
    public void HashPassword_ProducesBCryptFormat()
    {
        var hash = Svc().HashPassword("password");
        Assert.StartsWith("$2", hash); // BCrypt hashes always start with $2a$ or $2b$
    }
}
