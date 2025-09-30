using System.Collections.Generic;

namespace VirtoCommerce.Seo.Core.Models.Explain;

/// <summary>
/// Response returned by explain endpoints/services containing request context and optional pipeline explain results.
/// When <see cref="Results"/> is null it means no explain was produced (e.g. invalid input or no candidates found).
/// </summary>
public record SeoExplainResponse(
    /// <summary>
    /// Gets the store identifier.
    /// </summary>
    string StoreId,

    /// <summary>
    /// Gets the language code.
    /// </summary>
    string LanguageCode,

    /// <summary>
    /// Gets the permalink.
    /// </summary>
    string Permalink,

    /// <summary>
    /// Gets the list of SEO explain results.
    /// </summary>
    IList<SeoExplainResult> Results
);
