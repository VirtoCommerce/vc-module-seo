using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.MySql.Extensions;
using VirtoCommerce.Platform.Data.PostgreSql.Extensions;
using VirtoCommerce.Platform.Data.SqlServer.Extensions;
using VirtoCommerce.Seo.Core;
using VirtoCommerce.Seo.Core.Extensions;
using VirtoCommerce.Seo.Core.Services;
using VirtoCommerce.Seo.Data.MySql;
using VirtoCommerce.Seo.Data.PostgreSql;
using VirtoCommerce.Seo.Data.Repositories;
using VirtoCommerce.Seo.Data.Services;
using VirtoCommerce.Seo.Data.SqlServer;

namespace VirtoCommerce.Seo.Web;

public class Module : IModule, IHasConfiguration
{
    public ManifestModuleInfo ModuleInfo { get; set; }
    public IConfiguration Configuration { get; set; }

    public void Initialize(IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<SeoDbContext>(options =>
        {
            var databaseProvider = Configuration.GetValue("DatabaseProvider", "SqlServer");
            var connectionString = Configuration.GetConnectionString(ModuleInfo.Id) ?? Configuration.GetConnectionString("VirtoCommerce");

            switch (databaseProvider)
            {
                case "MySql":
                    options.UseMySqlDatabase(connectionString, typeof(MySqlDataAssemblyMarker), Configuration);
                    break;
                case "PostgreSql":
                    options.UsePostgreSqlDatabase(connectionString, typeof(PostgreSqlDataAssemblyMarker), Configuration);
                    break;
                default:
                    options.UseSqlServerDatabase(connectionString, typeof(SqlServerDataAssemblyMarker), Configuration);
                    break;
            }
        });

        var priorities = Configuration.GetSection("Seo:SeoInfoResolver:ObjectTypePriority").Get<string[]>();
        if (priorities != null)
        {
            // unknown object types should have the lowest priority
            // so, the array should be reversed to have the lowest priority at the beginning of the array
            SeoExtensions.OrderedObjectTypes = priorities.Reverse().ToArray();
        }

        serviceCollection.AddTransient<ISeoDuplicatesDetector, NullSeoDuplicateDetector>();
        serviceCollection.AddTransient<ICompositeSeoResolver, CompositeSeoResolver>();
        serviceCollection.AddTransient<IBrokenLinksRepository, BrokenLinksRepository>();
        serviceCollection.AddSingleton<Func<IBrokenLinksRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<IBrokenLinksRepository>());

        serviceCollection.AddTransient<IBrokenLinkSearchService, BrokenLinkSearchService>();
        serviceCollection.AddTransient<IBrokenLinkService, BrokenLinkService>();
        serviceCollection.AddTransient<ISeoFallbackHandler, SeoFallbackHandler>();
    }

    public void PostInitialize(IApplicationBuilder appBuilder)
    {
        var serviceProvider = appBuilder.ApplicationServices;

        // Register settings
        var settingsRegistrar = serviceProvider.GetRequiredService<ISettingsRegistrar>();
        settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);

        // Register permissions
        var permissionsRegistrar = serviceProvider.GetRequiredService<IPermissionsRegistrar>();
        permissionsRegistrar.RegisterPermissions(ModuleInfo.Id, "Seo", ModuleConstants.Security.Permissions.AllPermissions);

        // Apply migrations
        using var serviceScope = serviceProvider.CreateScope();
        using var dbContext = serviceScope.ServiceProvider.GetRequiredService<SeoDbContext>();
        dbContext.Database.Migrate();
    }

    public void Uninstall()
    {
        // Nothing to do here
    }
}
