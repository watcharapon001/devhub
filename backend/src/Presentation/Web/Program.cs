using Application.Common.Interfaces;
using Application.Projects.Queries;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Auth;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Web.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetProjectsQuery>());
builder.Services.AddInfrastructure(builder.Configuration);

// CORS (dev)
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// JWT
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    if (!await db.Users.AnyAsync())
    {
        db.Users.Add(new User
        {
            Username = "admin",
            PasswordHash = UserAuth.Hash("admin123"),
            Roles = "Admin"
        });
        await db.SaveChangesAsync();
    }
}

app.UseSwagger(); app.UseSwaggerUI();
app.UseCors();
app.UseDefaultFiles(); app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

// Login
app.MapPost("/api/auth/login", async (AuthLoginRequest req, IUserAuth userAuth, ITokenService tokens, CancellationToken ct) =>
{
    var found = await userAuth.FindByUsernameAsync(req.Username, ct);
    if (found is null)
        return Results.Problem(title: "Invalid username or password", statusCode: StatusCodes.Status401Unauthorized);

    var (id, username, roles) = found.Value;
    var ok = await userAuth.VerifyPasswordAsync(id, req.Password, ct);
    if (!ok)
        return Results.Problem(title: "Invalid username or password", statusCode: StatusCodes.Status401Unauthorized);


    var rolesArr = string.IsNullOrWhiteSpace(roles) ? null : roles.Split(',', StringSplitOptions.TrimEntries);
    var (jwt, exp) = tokens.CreateToken(id.ToString(), username, rolesArr);
    return Results.Ok(new AuthLoginResponse(jwt, exp, username));
}).AllowAnonymous();

// Public
app.MapGet("/api/health", () => Results.Ok(new { status = "ok", at = DateTime.UtcNow }));

// Protected
app.MapGet("/api/projects", [Authorize] async (IMediator mediator) =>
    Results.Ok(await mediator.Send(new GetProjectsQuery())));

app.MapFallbackToFile("index.html");
app.Run();
