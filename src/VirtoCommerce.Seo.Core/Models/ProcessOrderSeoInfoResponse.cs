namespace VirtoCommerce.Seo.Core.Models;

public class ProcessOrderSeoInfoResponse
{
    protected string Description { get; set; } = "Order of get best matching seo info.";
    public SeoInfosResponse FoundSeoInfos { get; set; }
    public SeoInfosResponse FilteredSeoInfos { get; set; }
    public SeoInfoScoresResponse SelectedSeoInfoScores { get; set; }
    public SeoInfoScoresResponse FilteredSeoInfoScores { get; set; }
    public SeoInfoScoresResponse OrderedSeoInfoScores { get; set; }
    public SeoInfosResponse SelectedSeoInfos { get; set; }
    public SeoInfoResponse SelectedSeoInfo { get; set; }
}
