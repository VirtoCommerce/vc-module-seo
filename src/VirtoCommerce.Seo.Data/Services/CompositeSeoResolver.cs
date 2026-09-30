using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Seo.Core.Events;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Models.Explain;
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
            .Where(HasObject)
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

    public virtual async Task<IList<SeoCandidate>> GetCandidatesAsync(SeoSearchCriteria criteria)
    {
        var candidateTasks = resolvers
            .Select(x => x.GetCandidatesAsync(criteria))
            .ToArray();

        return (await Task.WhenAll(candidateTasks))
            .SelectMany(x => x)
            .Where(x => HasObject(x.SeoInfo))
            .ToList();
    }

    private static bool HasObject(SeoInfo seoInfo)
    {
        return seoInfo.ObjectId != null && seoInfo.ObjectType != null;
    }
}
