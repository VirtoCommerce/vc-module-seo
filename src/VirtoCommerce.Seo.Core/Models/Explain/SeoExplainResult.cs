using System.Collections.Generic;

namespace VirtoCommerce.Seo.Core.Models.Explain;

public class SeoExplainResult(SeoExplainStage stage, IList<SeoExplainItem> items)
{
    public SeoExplainStage Stage { get; } = stage;

    public IList<SeoExplainItem> Items { get; } = items ?? [];
}
