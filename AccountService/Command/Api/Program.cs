using AccountService.Command.Application.Commands;
using AccountService.Command.Application.DTOs;
using AccountService.Command.Application.Handlers;
using AccountService.Command.Domain;
using AccountService.Command.Infrastructure;
using Infrastructure.Api.Extensions;
using Infrastructure.Api.Messaging;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAccountCommandInfrastructure(builder.Configuration);
builder.Services.AddScoped<IPasswordHasher<Account>, PasswordHasher<Account>>();
builder.Services.AddHandlersFromAssemblies(typeof(CreateAccountHandler).Assembly);
builder.Services.AddScoped<ICommandHandler<CreateAccountCommand, CreateAccountResponse>, CreateAccountHandler>();
builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
