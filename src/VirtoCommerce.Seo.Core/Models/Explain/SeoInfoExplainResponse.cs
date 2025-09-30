using System.Collections.Generic;

namespace VirtoCommerce.Seo.Core.Models.Explain;

public record SeoInfoExplainResponse(
    string StoreId,
    string LanguageCode,
    string Permalink,
    IList<SeoInfoExplainResult> Results
);
