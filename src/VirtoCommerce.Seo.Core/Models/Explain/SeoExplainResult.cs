using System;
using System.Collections.Generic;
using VirtoCommerce.Seo.Core.Models.Explain.Enums;

namespace VirtoCommerce.Seo.Core.Models.Explain;

/// <summary>
/// Represents a snapshot of the SEO resolution pipeline at a single stage.
/// Contains a human-readable <see cref="Description"/> and the list of tuples with scoring data
/// for candidates at this stage.
/// </summary>
public record SeoExplainResult
{
    /// <summary>
    /// The pipeline stage represented by this result.
    /// </summary>
    public SeoExplainPipelineStage Stage { get; init; }

    /// <summary>
    /// A short textual description of the stage intended for diagnostics and tests.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// The list of tuples describing candidates after this stage. Each tuple contains the SeoInfo (may be null),
    /// the resolved object type priority and the numeric score.
    /// </summary>
    public IList<(SeoInfo SeoInfo, int ObjectTypePriority, int Score)> SeoInfoWithScoredList { get; }

    public SeoExplainResult(SeoExplainPipelineStage stage, IList<(SeoInfo SeoInfo, int ObjectTypePriority, int Score)> seoInfoWithScoredList)
    {
        Stage = stage;
        Description = stage switch
        {
            SeoExplainPipelineStage.Unknown => string.Empty,
            SeoExplainPipelineStage.Original => "Stage 1: Original - candidates found by the resolver.",
            SeoExplainPipelineStage.Filtered => "Stage 2: Filtered - candidates matching store and language rules.",
            SeoExplainPipelineStage.Scored => "Stage 3: Scored - calculate numeric scores and object type priorities.",
            SeoExplainPipelineStage.FilteredScore => "Stage 4: FilteredScore - keep candidates with positive score.",
            SeoExplainPipelineStage.Ordered => "Stage 5: Ordered - order by score and object type priority.",
            SeoExplainPipelineStage.Final => "Stage 6: Final - select first candidate as the resolved SeoInfo.",
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };
        SeoInfoWithScoredList = seoInfoWithScoredList ?? new List<(SeoInfo SeoInfo, int ObjectTypePriority, int Score)>();
    }
}
