namespace VirtoCommerce.Seo.Core.Models.Explain;

public class SeoExplainItem(SeoInfo seoInfo)
{
    public SeoInfo SeoInfo { get; } = seoInfo;
    public int ObjectTypePriority { get; set; } = -1;
    public int Score { get; set; }
}
