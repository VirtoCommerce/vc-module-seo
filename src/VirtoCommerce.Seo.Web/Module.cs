using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Platform.Core.Modularity;
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
        serviceCollection.AddTransient<CompositeSeoResolver>();
    }

    public void PostInitialize(IApplicationBuilder appBuilder)
    {
    }

    public void Uninstall()
    {
        // Nothing to do here
    }
}
