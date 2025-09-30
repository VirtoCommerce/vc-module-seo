using System.Threading.Tasks;
using VirtoCommerce.Seo.Core.Extensions;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Models.Explain;
using VirtoCommerce.Seo.Core.Services;

namespace VirtoCommerce.Seo.Data.Services;

public class SeoInfoExplainService(ICompositeSeoResolver compositeSeoResolver) : ISeoInfoExplainService
{
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

        var tuple = seoInfosFromCompositeResolver.GetSeoInfoExplain(storeId, storeDefaultLanguage, languageCode);

        var processOrder = new SeoInfoExplainResponse(storeId, languageCode, permalink, tuple.Results);

        return processOrder;
    }
}
