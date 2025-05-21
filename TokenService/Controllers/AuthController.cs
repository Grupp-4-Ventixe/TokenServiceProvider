using Microsoft.AspNetCore.Mvc;
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

    public AuthController(ITokenService tokenService)
    {
        _tokenService = tokenService;
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
}

public class TokenRequest
{
    public string? UserId { get; set; }
    public string? Role { get; set; }
}

