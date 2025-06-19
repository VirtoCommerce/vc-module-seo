using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Seo.Core.Events;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Services;

namespace VirtoCommerce.Seo.Data.Services;

public class CompositeSeoResolver(
    IEnumerable<ISeoResolver> resolvers,
    IEventPublisher eventPublisher)
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
            var infoNotFoundEvent = AbstractTypeFactory<SeoInfoNotFoundEvent>.TryCreateInstance();
            infoNotFoundEvent.Criteria = criteria;
            await eventPublisher.Publish(infoNotFoundEvent);
        }

        return result;
    }
}
