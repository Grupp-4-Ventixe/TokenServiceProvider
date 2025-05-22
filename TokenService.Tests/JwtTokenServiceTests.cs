using Microsoft.Extensions.Configuration;
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
}
