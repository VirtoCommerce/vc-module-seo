using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Models.Explain;

namespace VirtoCommerce.Seo.Core.Services;

public interface ISeoResolver
{
    Task<IList<SeoInfo>> FindSeoAsync(SeoSearchCriteria criteria);

    // The resolved candidates must be exactly what FindSeoAsync returns; the default adds no rejected records
    async Task<IList<SeoCandidate>> GetCandidatesAsync(SeoSearchCriteria criteria)
    {
        var seoInfos = await FindSeoAsync(criteria) ?? [];
        return seoInfos.Select(x => new SeoCandidate(x)).ToList();
    }
}
