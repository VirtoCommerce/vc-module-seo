namespace VirtoCommerce.Seo.Core.Models.Explain;

public class SeoExplainItem(SeoInfo seoInfo)
{
    public SeoInfo SeoInfo { get; } = seoInfo;
    public int ObjectTypePriority { get; set; }
    public int Score { get; set; }
}
