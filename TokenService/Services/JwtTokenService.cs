namespace TokenService.Services;

public class JwtTokenService : ITokenService
{
    public string GenerateToken(string userId, string role)
    {
        // Placeholder – riktig JWT kommer senare
        return $"Generated token for {userId} with role {role}";
    }
}
