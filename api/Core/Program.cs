using Core.DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Core;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("DashboardDatabase")
                               ?? throw new InvalidOperationException(
                                   "Connection string 'DashboardDatabase' not found.");

        // Add services to the container.
        builder.Services.AddDbContext<DataContext>(options => options.UseNpgsql(connectionString));
        builder.Services.AddControllers();
        builder.Services.AddAuthorization();
        builder.Services.AddIdentityApiEndpoints<IdentityUser>()
            .AddEntityFrameworkStores<DataContext>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        // app.UseHttpsRedirection();
        app.MapIdentityApi<IdentityUser>();
        app.UseAuthorization();
        app.MapControllers();

        await app.RunAsync();
    }
}