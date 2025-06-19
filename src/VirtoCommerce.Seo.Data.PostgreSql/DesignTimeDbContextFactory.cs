using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using VirtoCommerce.Seo.Data.Repositories;

namespace VirtoCommerce.Seo.Data.PostgreSql;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SeoDbContext>
{
    public SeoDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<SeoDbContext>();
        var connectionString = args.Length != 0 ? args[0] : "Server=localhost;Username=virto;Password=virto;Database=VirtoCommerce3;";

        builder.UseNpgsql(
            connectionString,
            options => options.MigrationsAssembly(typeof(PostgreSqlDataAssemblyMarker).Assembly.GetName().Name));

        return new SeoDbContext(builder.Options);
    }
}
