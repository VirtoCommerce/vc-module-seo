using System.Collections.Generic;

namespace VirtoCommerce.Seo.Core.Models.Explain;

public class SeoExplainResult(SeoExplainStage stage, IList<SeoExplainItem> seoExplainItems)
{
    public SeoExplainStage Stage { get; } = stage;

    public string Description { get; } = stage switch
    {
        SeoExplainStage.Undefined => string.Empty,
        SeoExplainStage.Original => "Stage 1: Original - candidates found by the resolver.",
        SeoExplainStage.Filtered => "Stage 2: Filtered - candidates matching store and language rules.",
        SeoExplainStage.Scored => "Stage 3: Scored - calculate numeric scores and object type priorities.",
        SeoExplainStage.FilteredScore => "Stage 4: FilteredScore - keep candidates with positive score.",
        SeoExplainStage.Ordered => "Stage 5: Ordered - order by score and object type priority.",
        SeoExplainStage.Final => "Stage 6: Final - select first candidate as the resolved SeoInfo.",
        _ => string.Empty,
    };

    public IList<SeoExplainItem> SeoExplainItems { get; } = seoExplainItems ?? [];
}
