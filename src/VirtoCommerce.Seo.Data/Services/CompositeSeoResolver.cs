using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Services;

namespace VirtoCommerce.Seo.Data.Services;

public class CompositeSeoResolver(
    IEnumerable<ISeoResolver> resolvers,
    IEnumerable<ISeoInfoNotFoundHandler> seoInfoHandlers)
    : ICompositeSeoResolver
{
    public virtual async Task<IList<SeoInfo>> FindSeoAsync(SeoSearchCriteria criteria)
    {
        var searchTasks = resolvers
            .Select(x => x.FindSeoAsync(criteria))
            .ToArray();

        var result = (await Task.WhenAll(searchTasks))
            .SelectMany(x => x)
            .Where(x => x.ObjectId != null && x.ObjectType != null)
            .Distinct()
            .ToList();

        if (result.Count == 0)
        {
            var handlerTasks = seoInfoHandlers.Select(x => x.Handle(criteria)).ToArray();
            await Task.WhenAll(handlerTasks);
        }

        return result;
    }
}
