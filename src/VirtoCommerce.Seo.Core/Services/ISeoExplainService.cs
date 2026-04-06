using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.Seo.Core.Models.Explain;

namespace VirtoCommerce.Seo.Core.Services;

public interface ISeoExplainService
{
    /// <summary>
    /// Produces an explain response for the given store, default language, requested language and permalink.
    /// The response is a list of per-stage snapshots that describe how the best matching <see cref="VirtoCommerce.Seo.Core.Models.SeoInfo"/> was selected.
    /// </summary>
    public Task<IList<SeoExplainResult>> ExplainAsync(string storeId, string organizationId, string storeDefaultLanguage, string languageCode, string permalink);
}
