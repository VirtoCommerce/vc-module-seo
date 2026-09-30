using System;
using System.Collections.Generic;
using System.Linq;
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
    private const int MaxCandidates = 100;

    public Task<IList<SeoExplainResult>> ExplainAsync(
        string storeId,
        string organizationId,
        string storeDefaultLanguage,
        string languageCode,
        string permalink)
    {
        ArgumentNullException.ThrowIfNull(permalink);
        return ExplainInternalAsync(storeId, organizationId, storeDefaultLanguage, languageCode, permalink);
    }

    private async Task<IList<SeoExplainResult>> ExplainInternalAsync(string storeId, string organizationId, string storeDefaultLanguage, string languageCode, string permalink)
    {
        var criteria = AbstractTypeFactory<SeoSearchCriteria>.TryCreateInstance();

        criteria.StoreId = storeId;
        criteria.LanguageCode = languageCode;
        criteria.OrganizationId = organizationId;
        criteria.Permalink = permalink.StartsWith("/")
            ? permalink.Substring(1)
            : permalink;
        criteria.Take = MaxCandidates;

        var candidates = await compositeSeoResolver.GetCandidatesAsync(criteria);

        if (candidates.Count == 0)
        {
            return [];
        }

        var seoInfos = candidates
            .Where(x => x.IsResolved)
            .Select(x => x.SeoInfo)
            .ToList();

        // Request explain snapshots explicitly so the response contains pipeline stages
        var (_, explainResults) = seoInfos.GetBestMatchingSeoInfo(storeId, organizationId, storeDefaultLanguage, languageCode, explain: true);

        return [new SeoExplainResult(SeoExplainStage.Candidates, candidates), .. explainResults];
    }
}
