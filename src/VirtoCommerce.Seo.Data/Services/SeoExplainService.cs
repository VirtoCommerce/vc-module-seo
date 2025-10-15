using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Seo.Core.Extensions;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Models.Explain;
using VirtoCommerce.Seo.Core.Services;

namespace VirtoCommerce.Seo.Data.Services;

/// <summary>
/// Service that executes the explain pipeline for a given store/language/permalink combination.
/// It relies on a composite resolver to fetch candidates and then delegates to <see cref="SeoExtensions.GetBestMatchingSeoInfo(IEnumerable{SeoInfo},string,string,string,bool)"/>.
/// </summary>
public class SeoExplainService(ICompositeSeoResolver compositeSeoResolver) : ISeoExplainService
{
    public Task<IList<SeoExplainResult>> ExplainAsync(
        string storeId,
        string storeDefaultLanguage,
        string languageCode,
        string permalink)
    {
        ArgumentNullException.ThrowIfNull(permalink);
        return ExplainInternalAsync(storeId, storeDefaultLanguage, languageCode, permalink);
    }

    private async Task<IList<SeoExplainResult>> ExplainInternalAsync(string storeId, string storeDefaultLanguage, string languageCode, string permalink)
    {
        var criteria = AbstractTypeFactory<SeoSearchCriteria>.TryCreateInstance();

        criteria.StoreId = storeId;
        criteria.LanguageCode = languageCode;
        criteria.Permalink = permalink.StartsWith("/")
            ? permalink.Substring(1)
            : permalink;

        var seoInfos = await compositeSeoResolver.FindSeoAsync(criteria);

        if (seoInfos.IsNullOrEmpty())
        {
            return [];
        }

        // Request explain snapshots explicitly so the response contains pipeline stages
        var (_, explainResults) = seoInfos.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, languageCode, explain: true);

        return explainResults ?? [];
    }
}
