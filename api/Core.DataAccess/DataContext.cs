using Core.Model;
using Microsoft.EntityFrameworkCore;

namespace Core.DataAccess;

public class DataContext : DbContext
{
    public DbSet<TaskCategory> TaskCategories { get; set; }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
    }
}