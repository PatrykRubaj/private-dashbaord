using Core.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Core;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("DashboardDatabase");
        
        // Add services to the container.
        builder.Services.AddDbContext<DataContext>(options => options.UseNpgsql(connectionString));
        builder.Services.AddControllers();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        
        app.Run();
    }
}
