using System.Threading.Tasks;
using VirtoCommerce.Seo.Core.Extensions;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Models.SlugInfo;
using VirtoCommerce.Seo.Core.Services;

namespace VirtoCommerce.Seo.Data.Services;

public class SlugExplainService(ICompositeSeoResolver compositeSeoResolver) : ISlugExplainService
{
    public async Task<SlugInfoResponse> GetExplainAsync(
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
            return new SlugInfoResponse(storeId, languageCode, permalink);
        }

        var results = seoInfosFromCompositeResolver.GetSeoInfosResponses(storeId, storeDefaultLanguage, languageCode);

        var processOrder = new SlugInfoResponse(storeId, languageCode, permalink, results);

        return processOrder;
    }
}
