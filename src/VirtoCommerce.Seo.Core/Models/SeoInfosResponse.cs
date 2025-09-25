namespace VirtoCommerce.Seo.Core.Models;

public class SeoInfosResponse(string description, SeoInfo[] seoInfos)
{
    public string Description { get; set; } = description;
    public SeoInfo[] SeoInfos { get; set; } = seoInfos;
}
