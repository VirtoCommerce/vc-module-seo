namespace VirtoCommerce.Seo.Core.Models;

public class SeoInfoResponse(string description, SeoInfo seoInfo)
{
    public string Description { get; set; } = description;
    public SeoInfo SeoInfo { get; set; } = seoInfo;
}
