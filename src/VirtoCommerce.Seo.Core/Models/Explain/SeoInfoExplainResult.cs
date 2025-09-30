using System;
using System.Collections.Generic;
using VirtoCommerce.Seo.Core.Models.Explain.Enums;

namespace VirtoCommerce.Seo.Core.Models.Explain;

/// <summary>
/// Represents a snapshot of the SEO resolution pipeline at a single stage.
/// Contains a human-readable <see cref="Description"/> and the list of tuples with scoring data
/// for candidates at this stage.
/// </summary>
public record SeoInfoExplainResult
{
    /// <summary>
    /// The pipeline stage represented by this result.
    /// </summary>
    public PipelineExplainStage Stage { get; init; }

    /// <summary>
    /// A short textual description of the stage intended for diagnostics and tests.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// The list of tuples describing candidates after this stage. Each tuple contains the SeoInfo (may be null),
    /// the resolved object type priority and the numeric score.
    /// </summary>
    public IList<(SeoInfo SeoInfo, int ObjectTypePriority, int Score)> SeoInfoWithScoredList { get; }

    public SeoInfoExplainResult(PipelineExplainStage stage, IList<(SeoInfo SeoInfo, int ObjectTypePriority, int Score)> seoInfoWithScoredList)
    {
        Stage = stage;
        Description = stage switch
        {
            PipelineExplainStage.Unknown => string.Empty,
            PipelineExplainStage.Original => "Stage 1: Original - candidates found by the resolver.",
            PipelineExplainStage.Filtered => "Stage 2: Filtered - candidates matching store and language rules.",
            PipelineExplainStage.Scored => "Stage 3: Scored - calculate numeric scores and object type priorities.",
            PipelineExplainStage.FilteredScore => "Stage 4: FilteredScore - keep candidates with positive score.",
            PipelineExplainStage.Ordered => "Stage 5: Ordered - order by score and object type priority.",
            PipelineExplainStage.Final => "Stage 6: Final - select first candidate as the resolved SeoInfo.",
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };
        SeoInfoWithScoredList = seoInfoWithScoredList ?? new List<(SeoInfo SeoInfo, int ObjectTypePriority, int Score)>();
    }
}
