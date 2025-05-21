using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using TokenService.Services;

namespace TokenService.Controllers;

/* Denna controller genererades med hjälp av ChatGPT-4o
   Syfte: Tar emot ett userId och role via POST /auth/token,
   validerar input, och använder ITokenService för att generera en JWT-token.
   TokenRequest-klassen används som modell för inkommande data. */
[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _config;


    public AuthController(ITokenService tokenService, IConfiguration config)
    {
        _tokenService = tokenService;
        _config = config;
    }

    [HttpPost("token")]
    public IActionResult GenerateToken([FromBody] TokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.Role))
        {
            return BadRequest("UserId and Role are required.");
        }

        var token = _tokenService.GenerateToken(request.UserId, request.Role);
        return Ok(new { token });
    }

    /* Denna metod genererades med hjälp av ChatGPT-4o
   Syfte: Validerar en JWT-token som skickas via Authorization-headern i ett GET-anrop.
   1. Läser token från "Authorization: Bearer ..." header
   2. Hämtar JWT-inställningar från konfiguration (hemlig nyckel, issuer, audience)
   3. Använder JwtSecurityTokenHandler för att validera signaturen, giltighet, issuer och audience
   4. Returnerar 200 OK om token är giltig, annars 401 Unauthorized */

    [HttpGet("validate")]
    public IActionResult ValidateToken()
    {
        var authHeader = Request.Headers["Authorization"].ToString();

        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            return Unauthorized("Missing or invalid Authorization header.");
        }

        var token = authHeader.Substring("Bearer ".Length);

        var secret = _config["Jwt:Key"];
        var issuer = _config["Jwt:Issuer"];
        var audience = _config["Jwt:Audience"];

        if (string.IsNullOrEmpty(secret))
        {
            return StatusCode(500, "JWT secret key is not configured.");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secret);

        var validationParams = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            tokenHandler.ValidateToken(token, validationParams, out SecurityToken validatedToken);
            return Ok("Token is valid.");
        }
        catch
        {
            return Unauthorized("Token is invalid.");
        }
    }
}

public class TokenRequest
{
    public string? UserId { get; set; }
    public string? Role { get; set; }
}

