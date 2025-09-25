namespace VirtoCommerce.Seo.Core.Models;

public class SeoInfoScored
{
    public SeoInfo SeoRecord { get; set; } // ToDo: Named SeoRecord to maintain backward compatibility with the old code, but it would be better if the name reflected the essence. (SeoInfo)
    public int ObjectTypePriority { get; set; }
    public int Score { get; set; }
}
