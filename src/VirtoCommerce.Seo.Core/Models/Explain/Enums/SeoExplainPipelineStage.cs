namespace VirtoCommerce.Seo.Core.Models.Explain.Enums;

/// <summary>
/// Enumerates the stages of the SEO resolution pipeline used for producing explain information.
/// </summary>
public enum SeoExplainPipelineStage
{
    /// <summary>
    /// Unknown or uninitialized stage.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Original: candidates returned by the resolver before any filtering or scoring.
    /// </summary>
    Original = 1,

    /// <summary>
    /// Filtered: candidates that passed basic store/language filtering rules.
    /// </summary>
    Filtered = 2,

    /// <summary>
    /// Scored: numeric scoring and object type priorities have been calculated.
    /// </summary>
    Scored = 3,

    /// <summary>
    /// FilteredScore: candidates with non-positive scores have been removed.
    /// </summary>
    FilteredScore = 4,

    /// <summary>
    /// Ordered: candidates ordered by score and object type priority.
    /// </summary>
    Ordered = 5,

    /// <summary>
    /// Final: a single best candidate (or none) selected from the ordered list.
    /// </summary>
    Final = 6,
}
