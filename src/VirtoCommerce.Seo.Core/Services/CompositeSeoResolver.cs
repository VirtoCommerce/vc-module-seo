using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Seo.Core.Models;

namespace VirtoCommerce.Seo.Core.Services;

public class CompositeSeoResolver(
    IEnumerable<ISeoResolver> resolvers)
    : ISeoResolver
{
    public async Task<IList<SeoInfo>> FindSeoAsync(SeoSearchCriteria criteria)
    {
        var searchTasks = resolvers.Select(x => x.FindSeoAsync(criteria)).ToArray();
        var result = (await Task.WhenAll(searchTasks)).SelectMany(x => x).Where(x => x.ObjectId != null && x.ObjectType != null).Distinct().ToList();
        return result;
    }
}

