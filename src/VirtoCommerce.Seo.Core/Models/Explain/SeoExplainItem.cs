namespace VirtoCommerce.Seo.Core.Models.Explain;

public class SeoExplainItem(SeoInfo seoInfo, int objectTypePriority = -1, int score = 0)
{
    public SeoInfo SeoInfo { get; } = seoInfo;
    public int ObjectTypePriority { get; set; } = objectTypePriority;
    public int Score { get; set; } = score;
}
