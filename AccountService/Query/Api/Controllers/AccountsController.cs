using AccountService.Query.Domain;
using AccountService.Query.Infrastructure;
using Infrastructure.Api.Common;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace AccountService.Query.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly ReadDbContext _readDb;
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(ReadDbContext readDb, ILogger<AccountsController> logger)
    {
        _readDb = readDb;
        _logger = logger;
    }

    /// <summary>
    /// Get all accounts from MongoDB read model
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllAccounts()
    {
        try
        {
            var accounts = await _readDb.Accounts.Find(_ => true).ToListAsync();
            return Ok(BaseResponse<List<AccountReadModel>>.Ok(accounts));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching accounts");
            return StatusCode(500, BaseResponse<object>.Fail("Error fetching accounts"));
        }
    }

    /// <summary>
    /// Get account by ID
    /// </summary>
    [HttpGet("{id}", Name = "GetAccountById")]
    public async Task<IActionResult> GetAccountById(Guid id)
    {
        try
        {
            var account = await _readDb.Accounts.Find(a => a.Id == id).FirstOrDefaultAsync();

            if (account == null)
                return NotFound(BaseResponse<object>.Fail("Account not found"));

            return Ok(BaseResponse<AccountReadModel>.Ok(account));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching account");
            return StatusCode(500, BaseResponse<object>.Fail("Error fetching account"));
        }
    }
}
