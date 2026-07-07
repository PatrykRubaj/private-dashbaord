using Core.DataAccess;
using Core.JsonConverters;
using Core.Model.Options;
using Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;

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
        builder.Services.AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new FlexibleDecimalConverter()));
        builder.Services.AddAuthorization();
        builder.Services.AddIdentityApiEndpoints<IdentityUser>()
            .AddEntityFrameworkStores<DataContext>();
        builder.Services.AddHttpClient<IAirGradientClient, AirGradientClient>();
        builder.Services.Configure<AirGradientOptions>(
            builder.Configuration.GetSection(AirGradientOptions.SectionName));
        builder.Services.AddHostedService<AirGradientPollingService>();
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        // app.UseHttpsRedirection();
        app.MapIdentityApi<IdentityUser>();
        app.UseAuthorization();
        app.MapControllers();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        await app.RunAsync();
    }
}