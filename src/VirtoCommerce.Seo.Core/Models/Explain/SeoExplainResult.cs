using System.Collections.Generic;

namespace VirtoCommerce.Seo.Core.Models.Explain;

public class SeoExplainResult(SeoExplainStage stage, IList<SeoExplainItem> seoExplainItems)
{
    public SeoExplainStage Stage { get; } = stage;

    public IList<SeoExplainItem> SeoExplainItems { get; } = seoExplainItems ?? [];
}
