using AccountService.Command.Api.Authentication;
using AccountService.Command.Application.Abstractions;
using AccountService.Command.Application.Commands;
using AccountService.Command.Application.DTOs;
using AccountService.Command.Application.Handlers;
using AccountService.Command.Domain;
using AccountService.Command.Infrastructure;
using Infrastructure.Api.Extensions;
using Infrastructure.Api.Middleware;
using Infrastructure.Api.Messaging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));

var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
if (string.IsNullOrWhiteSpace(jwtOptions.SecretKey))
{
    throw new InvalidOperationException("JWT secret key is not configured.");
}

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.Zero,
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "WorkForceHub Account Command API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter a valid JWT bearer token.",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                },
            },
            Array.Empty<string>()
        }
    });
});
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
