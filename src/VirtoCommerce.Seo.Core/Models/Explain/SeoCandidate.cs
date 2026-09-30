using System.Collections.Generic;

namespace VirtoCommerce.Seo.Core.Models.Explain;

public class SeoCandidate(SeoInfo seoInfo) : SeoExplainItem(seoInfo)
{
    public IList<SeoCandidateReason> Reasons { get; } = [];

    public bool IsResolved => Reasons.Count == 0;
}
