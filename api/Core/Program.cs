namespace Core;

using DataAccess;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("DashboardDatabase") 
                               ?? throw new InvalidOperationException("Connection string 'DashboardDatabase' not found.");
        
        // Add services to the container.
        builder.Services.AddDbContext<DataContext>(options => options.UseNpgsql(connectionString));
        builder.Services.AddControllers();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        // app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        
        await app.RunAsync();
    }
}
