using System.Collections.Generic;

namespace VirtoCommerce.Seo.Core.Models.Explain;

public class SeoExplainItem(SeoInfo seoInfo)
{
    public SeoInfo SeoInfo { get; } = seoInfo;
    public int ObjectTypePriority { get; set; } = -1;
    public int Score { get; set; }
    public IList<SeoCandidateReason> Reasons { get; } = [];
    public bool IsResolved => Reasons.Count == 0;
}
