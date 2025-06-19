using System.Reflection;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Data.Extensions;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Seo.Data.Models;

namespace VirtoCommerce.Seo.Data.Repositories;

public class SeoDbContext : DbContextBase
{
    public SeoDbContext(DbContextOptions<SeoDbContext> options)
        : base(options)
    {
    }

    protected SeoDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BrokenLinkEntity>().ToAuditableEntityTable("BrokenLink");
        modelBuilder.Entity<BrokenLinkEntity>().HasIndex(x => new { x.Permalink, x.StoreId, x.LanguageCode }).IsUnique();

        switch (Database.ProviderName)
        {
            case "Pomelo.EntityFrameworkCore.MySql":
                modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.Seo.Data.MySql"));
                break;
            case "Npgsql.EntityFrameworkCore.PostgreSQL":
                modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.Seo.Data.PostgreSql"));
                break;
            case "Microsoft.EntityFrameworkCore.SqlServer":
                modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.Seo.Data.SqlServer"));
                break;
        }
    }
}
