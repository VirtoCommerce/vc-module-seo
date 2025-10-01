using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.Seo.Core.Extensions;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Models.Explain;
using VirtoCommerce.Seo.Core.Services;

namespace VirtoCommerce.Seo.Data.Services;

/// <summary>
/// Service that executes the explain pipeline for a given store/language/permalink combination.
/// It relies on a composite resolver to fetch candidates and then delegates to <see cref="SeoExtensions.ExplainBestMatchingSeoInfo"/>.
/// </summary>
public class SeoExplainService(ICompositeSeoResolver compositeSeoResolver) : ISeoExplainService
{
    public async Task<IList<SeoExplainResult>> ExplainAsync(
        string storeId,
        string storeDefaultLanguage,
        string languageCode,
        string permalink)
    {
        var criteria = new SeoSearchCriteria()
        {
            StoreId = storeId,
            LanguageCode = languageCode,
            Permalink = permalink
        };

        var seoInfosFromCompositeResolver = await compositeSeoResolver.FindSeoAsync(criteria);

        if (seoInfosFromCompositeResolver == null || seoInfosFromCompositeResolver.Count == 0)
        {
            return new List<SeoExplainResult>();
        }

        // Request explain snapshots explicitly so the response contains pipeline stages
        var explain = seoInfosFromCompositeResolver.ExplainBestMatchingSeoInfo(storeId, storeDefaultLanguage, languageCode, withExplain: true);

        return explain.Results;
    }
}
