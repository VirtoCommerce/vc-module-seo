using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Seo.Core;
using VirtoCommerce.Seo.Core.Extensions;
using VirtoCommerce.Seo.Core.Services;
using VirtoCommerce.Seo.Data.Services;

namespace VirtoCommerce.Seo.Web;

public class Module : IModule, IHasConfiguration
{
    public ManifestModuleInfo ModuleInfo { get; set; }
    public IConfiguration Configuration { get; set; }

    public void Initialize(IServiceCollection serviceCollection)
    {
        var priorities = Configuration.GetSection("Seo:SeoInfoResolver:ObjectTypePriority").Get<string[]>();
        if (priorities != null)
        {
            // unknown object types should have the lowest priority
            // so, the array should be reversed to have the lowest priority at the beginning of the array
            SeoExtensions.OrderedObjectTypes = priorities.Reverse().ToArray();
        }

        serviceCollection.AddTransient<ISeoDuplicatesDetector, NullSeoDuplicateDetector>();
        serviceCollection.AddTransient<ICompositeSeoResolver, CompositeSeoResolver>();
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
    }

    public void Uninstall()
    {
        // Nothing to do here
    }
}
