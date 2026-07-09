using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Pgvector.EntityFrameworkCore;

namespace ProjectNetIa.Infrastructure.Data;

public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = "Host=localhost;Port=5433;Database=ProjectNetIaDb;Username=postgres;Password=1234";

        optionsBuilder.UseNpgsql(connectionString, options => options.UseVector());

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
