using AccountService.Command.Api.Authentication;
using AccountService.Command.Application.Abstractions;
using AccountService.Command.Application.Commands;
using AccountService.Command.Application.DTOs;
using AccountService.Command.Application.Handlers;
using AccountService.Command.Domain;
using AccountService.Command.Infrastructure;
using Infrastructure.Api.Authentication;
using Infrastructure.Api.Extensions;
using Infrastructure.Api.Middleware;
using Infrastructure.Api.Messaging;
using Microsoft.AspNetCore.Identity;
using AccountService.Command.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddWorkForceHubJwtAuthentication(builder.Configuration);
builder.Services.AddWorkForceHubSwagger("WorkForceHub Account Command API");
builder.Services.AddAccountCommandInfrastructure(builder.Configuration);
builder.Services.AddScoped<IPasswordHasher<Account>, PasswordHasher<Account>>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddHandlersFromAssemblies(typeof(CreateAccountHandler).Assembly);
builder.Services.AddScoped<ICommandHandler<CreateAccountCommand, CreateAccountResponse>, CreateAccountHandler>();
builder.Services.AddScoped<ICommandHandler<LoginCommand, LoginResponse>, LoginHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateAccountCommand, AccountResponse>, UpdateAccountHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateAccountRoleCommand, AccountResponse>, UpdateAccountRoleHandler>();
builder.Services.AddScoped<ICommandHandler<ChangeAccountPasswordCommand, bool>, ChangeAccountPasswordHandler>();
builder.Services.AddScoped<ICommandHandler<DeleteAccountCommand, bool>, DeleteAccountHandler>();
builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();

var app = builder.Build();

await app.ApplyMigrationsAsync<AccountCommandDbContext>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalErrorHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
