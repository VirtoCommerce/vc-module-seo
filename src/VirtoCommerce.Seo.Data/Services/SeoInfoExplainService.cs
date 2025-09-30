using System.Threading.Tasks;
using VirtoCommerce.Seo.Core.Extensions;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Models.Explain;
using VirtoCommerce.Seo.Core.Services;

namespace VirtoCommerce.Seo.Data.Services;

/// <summary>
/// Service that executes the explain pipeline for a given store/language/permalink combination.
/// It relies on a composite resolver to fetch candidates and then delegates to <see cref="SeoExtensions.GetSeoInfoExplain"/>.
/// </summary>
public class SeoInfoExplainService(ICompositeSeoResolver compositeSeoResolver) : ISeoInfoExplainService
{
    /// <summary>
    /// Gather candidates using <see cref="ICompositeSeoResolver"/> and run the explainable pipeline.
    /// Returns a <see cref="SeoInfoExplainResponse"/> containing the original request context and explain results when available.
    /// If the resolver returns null or an empty list the response contains a null <see cref="SeoInfoExplainResponse.Results"/>.
    /// </summary>
    public async Task<SeoInfoExplainResponse> GetSeoInfoExplainAsync(
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
            return new SeoInfoExplainResponse(storeId, languageCode, permalink, null);
        }

        // Request explain snapshots explicitly so the response contains pipeline stages
        var tuple = seoInfosFromCompositeResolver.GetSeoInfoExplain(storeId, storeDefaultLanguage, languageCode, withExplain: true);

        var response = new SeoInfoExplainResponse(storeId, languageCode, permalink, tuple.Results);

        return response;
    }
}
