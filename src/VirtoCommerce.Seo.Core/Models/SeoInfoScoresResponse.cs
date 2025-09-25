namespace VirtoCommerce.Seo.Core.Models;

public class SeoInfoScoresResponse(string description, SeoInfoScored[] seoInfoScores)
{
    public string Description { get; set; } = description;
    public SeoInfoScored[] SeoInfoScores { get; set; } = seoInfoScores;
}
