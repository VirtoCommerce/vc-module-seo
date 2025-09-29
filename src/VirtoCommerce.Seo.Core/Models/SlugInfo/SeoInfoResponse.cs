namespace VirtoCommerce.Seo.Core.Models.SlugInfo;

/// <summary>
/// Represents evaluation result for a single SeoInfo during matching process.
/// Immutable to avoid accidental modifications after creation.
/// </summary>
public sealed record SeoInfoResponse
{
    public SeoInfo SeoInfo { get; init; }
    public int ObjectTypePriority { get; init; }
    public int Score { get; init; }

    public SeoInfoResponse() { }

    public SeoInfoResponse(SeoInfo seoInfo, int objectTypePriority, int score)
    {
        SeoInfo = seoInfo;
        ObjectTypePriority = objectTypePriority;
        Score = score;
    }
}
