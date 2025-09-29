using System.Collections.Generic;

namespace VirtoCommerce.Seo.Core.Models.SlugInfo;

public class SlugInfoResponse(string storeId, string languageCode, string permalink, IList<SeoInfosResponse> results)
{
    public SlugInfoResponse(string storeId, string languageCode, string permalink)
        : this(storeId, languageCode, permalink, new List<SeoInfosResponse>())
    {
    }

    public string StoreId { get; init; } = storeId;
    public string LanguageCode { get; init; } = languageCode;
    public string Permalink { get; init; } = permalink;
    public IList<SeoInfosResponse> Results { get; init; } = results;
}
