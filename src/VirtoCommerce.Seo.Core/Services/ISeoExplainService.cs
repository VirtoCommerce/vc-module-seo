using System.Threading.Tasks;
using VirtoCommerce.Seo.Core.Models.Explain;

namespace VirtoCommerce.Seo.Core.Services;

public interface ISeoExplainService
{
    /// <summary>
    /// Produce an explain response for the given store, default language, requested language and permalink.
    /// The response contains the original request context and, when available, per-stage pipeline snapshots that
    /// describe how the best matching <see cref="VirtoCommerce.Seo.Core.Models.SeoInfo"/> was selected.
    /// </summary>
    /// <param name="storeId">Store identifier used to scope results. May be null in which case no explain will be produced.</param>
    /// <param name="storeDefaultLanguage">Default language of the store used as a fallback when requested language is not available.</param>
    /// <param name="languageCode">Requested language code for selection (may be null).</param>
    /// <param name="permalink">Requested permalink/slug used when resolving candidates.</param>
    /// <returns>A task that returns a <see cref="SeoExplainResponse"/> containing request context and optional explain results.</returns>
    public Task<SeoExplainResponse> GetSeoExplainAsync(string storeId, string storeDefaultLanguage, string languageCode, string permalink);
}
