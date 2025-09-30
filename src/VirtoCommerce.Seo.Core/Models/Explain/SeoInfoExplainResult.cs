using System;
using System.Collections.Generic;
using VirtoCommerce.Seo.Core.Models.Explain.Enums;

namespace VirtoCommerce.Seo.Core.Models.Explain;

public record SeoInfoExplainResult
{
    public PipelineExplainStage Stage { get; init; }
    public string Description { get; }
    public IList<(SeoInfo SeoInfo, int ObjectTypePriority, int Score)> SeoInfoWithScoredList { get; }

    public SeoInfoExplainResult(PipelineExplainStage stage, IList<(SeoInfo SeoInfo, int ObjectTypePriority, int Score)> seoInfoWithScoredList)
    {
        Stage = stage;
        Description = stage switch
        {
            PipelineExplainStage.Unknown => "",
            PipelineExplainStage.Original => "Stage 1: Original found by SeoInfo.",
            PipelineExplainStage.Filtered => "Stage 2: Filtering is there seo.",
            PipelineExplainStage.Scored => "Stage 3: Calculate scores.",
            PipelineExplainStage.FilteredScore => "Stage 4: Filter score greater than 0.",
            PipelineExplainStage.Ordered => "Stage 5: Order by score, then order by desc objectTypePriority.",
            PipelineExplainStage.Final => "Stage 6: Select first or default SeoInfo.",
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };
        SeoInfoWithScoredList = seoInfoWithScoredList ?? new List<(SeoInfo SeoInfo, int ObjectTypePriority, int Score)>();
    }
}
