using Application.Common.Interfaces;
using Infrastructure.Auth;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var cs = config.GetConnectionString("Postgres")
                 ?? Environment.GetEnvironmentVariable("DB_CONNECTION");
        services.AddDbContext<AppDbContext>(o => o.UseNpgsql(cs));
        services.AddScoped<IAppDb>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IUserAuth, UserAuth>();
        return services;
    }
}
