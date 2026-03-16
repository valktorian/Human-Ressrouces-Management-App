using AccountService.Command.Application.Commands;
using AccountService.Command.Application.DTOs;
using Infrastructure.Api.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Command.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ICommandDispatcher _dispatcher;

    public AuthController(ICommandDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <summary>
    /// Authenticate an account and return a JWT access token.
    /// </summary>
    /// <param name="command">The login credentials.</param>
    /// <param name="ct">The request cancellation token.</param>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
    {
        var response = await _dispatcher.SendAsync<LoginCommand, LoginResponse>(command, ct);
        return Ok(response);
    }
}
