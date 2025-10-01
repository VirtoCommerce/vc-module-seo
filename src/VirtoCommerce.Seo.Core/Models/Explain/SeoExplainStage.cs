namespace VirtoCommerce.Seo.Core.Models.Explain;

/// <summary>
/// Enumerates the stages of the SEO resolution pipeline used for producing explain information.
/// </summary>
public enum SeoExplainStage
{
    /// <summary>
    /// Undefined or uninitialized stage.
    /// </summary>
    Undefined,

    /// <summary>
    /// Original: candidates returned by the resolver before any filtering or scoring.
    /// </summary>
    Original,

    /// <summary>
    /// Filtered: candidates that passed basic store/language filtering rules.
    /// </summary>
    Filtered,

    /// <summary>
    /// Scored: numeric scoring and object type priorities have been calculated.
    /// </summary>
    Scored,

    /// <summary>
    /// FilteredScore: candidates with a positive score only.
    /// </summary>
    FilteredScore,

    /// <summary>
    /// Ordered: candidates ordered by score and object type priority.
    /// </summary>
    Ordered,

    /// <summary>
    /// Final: a single best candidate (or none) selected from the ordered list.
    /// </summary>
    Final,
}
