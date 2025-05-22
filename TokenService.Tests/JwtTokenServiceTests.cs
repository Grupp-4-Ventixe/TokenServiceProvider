using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TokenService.Services;

namespace TokenService.Tests;

public class JwtTokenServiceTests
{
    [Fact]
    public void GenerateToken_ShouldReturnToken_WhenInputIsValid()
    {
        // Arrange
        var configData = new Dictionary<string, string?>
        {
            { "Jwt:Key", "test_secret_key_12345678901011_!" },
            { "Jwt:Issuer", "test-service" },
            { "Jwt:Audience", "test-clients" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        var tokenService = new JwtTokenService(configuration);

        // Act
        var token = tokenService.GenerateToken("testuser", "admin");

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public void GenerateToken_ShouldThrowException_WhenKeyisMissing()
    {
        // Arrange
        var configData = new Dictionary<string, string?>()
        {
            { "Jwt:Issuer", "test-issuer" },
            { "Jwt:Audience", "test-audience" }
        };

        var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(configData)
        .Build();

        var tokenService = new JwtTokenService(configuration);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
        {
            tokenService.GenerateToken("testuser", "admin");
        });
    }

    [Fact]
    public void GenerateToken_ShouldContainExpectedClaims()
    {
        // Arrange
        var configData = new Dictionary<string, string?>()
        {
            { "Jwt:Key", "test_secret_key_1234567890_long_enough!" },
            { "Jwt:Issuer", "test-issuer" },
            { "Jwt:Audience", "test-audience" }
        };

        var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(configData)
        .Build();

        var tokenService = new JwtTokenService(configuration);

        var userId = "testuser";
        var role = "admin";

        // Act
        var token = tokenService.GenerateToken(userId, role);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Assert
        Assert.Contains(jwtToken.Claims, c => c.Type == "sub" && c.Value == userId);
        Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Role && c.Value == role);
        Assert.Equal("test-issuer", jwtToken.Issuer);
        Assert.Equal("test-audience", jwtToken.Audiences.First());
    }
}
