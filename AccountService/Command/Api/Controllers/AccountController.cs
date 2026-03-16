using AccountService.Command.Application.Commands;
using AccountService.Command.Application.DTOs;
using Infrastructure.Api.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Command.Api.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountController : ControllerBase
{
    private readonly ICommandDispatcher _dispatcher;

    public AccountController(ICommandDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAccountCommand command, CancellationToken ct)
    {
        var response = await _dispatcher.SendAsync<CreateAccountCommand, CreateAccountResponse>(command, ct);
        return Ok(response);
    }
}
