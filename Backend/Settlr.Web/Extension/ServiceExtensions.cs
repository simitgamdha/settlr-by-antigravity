using Microsoft.EntityFrameworkCore;
using Settlr.Common.Helper;
using Settlr.Data.DbContext;
using Settlr.Data.IRepositories;
using Settlr.Data.Repositories;
using Settlr.Services.IServices;
using Settlr.Services.Services;

namespace Settlr.Web.Extension;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        string connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured");
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // JWT Helper
        string secretKey = configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");
        string issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured");
        string audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience is not configured");
        int expirationMinutes = int.Parse(configuration["Jwt:ExpirationMinutes"] ?? "60");

        services.AddSingleton(new JwtHelper(secretKey, issuer, audience, expirationMinutes));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IGroupMemberRepository, GroupMemberRepository>();
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        services.AddScoped<IExpenseSplitRepository, ExpenseSplitRepository>();

        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<IDashboardService, DashboardService>();

        // AutoMapper
        services.AddAutoMapper(typeof(Program));

        return services;
    }
}
