using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Seo.Core.Extensions;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Services;

namespace VirtoCommerce.Seo.Data.Services;

public class MaintenanceService(ICompositeSeoResolver compositeSeoResolver) : IMaintenanceService
{
    private const string StoreDefaultLanguage = "en-US";
    private const string Stage0Description = "Stage 0: Find SeoInfos from compositeResolver.";
    private const string Stage1Description = "Stage 1: Filtering is there seo.";
    private const string Stage2Description = "Stage 2: Select SeoInfo, ObjectTypePriority, Score.";
    private const string Stage3Description = "Stage 3: Filter score greater than 0.";
    private const string Stage4Description = "Stage 4: Order by score, then order by desc ObjectTypePriority.";
    private const string Stage5Description = "Stage 5: Select SeoInfos.";
    private const string Stage6Description = "Stage 6: Select first or default SeoInfo.";

    public async Task<ProcessOrderSeoInfoResponse> GetSeoInfoForTestAsync(string storeId, string languageCode, string permalink, CancellationToken cancellationToken)
    {
        var criteria = new SeoSearchCriteria()
        {
            StoreId = storeId,
            LanguageCode = languageCode,
            Permalink = permalink
        };

        var seoInfosFromCompositeResolver = await compositeSeoResolver.FindSeoAsync(criteria);

        if (seoInfosFromCompositeResolver == null || seoInfosFromCompositeResolver.Count == 0)
        {
            return new ProcessOrderSeoInfoResponse
            {
                FoundSeoInfos = new SeoInfosResponse(Stage0Description, null),
                FilteredSeoInfos = new SeoInfosResponse(Stage1Description, null),
                SelectedSeoInfoScores = new SeoInfoScoresResponse(Stage2Description, null),
                FilteredSeoInfoScores = new SeoInfoScoresResponse(Stage3Description, null),
                OrderedSeoInfoScores = new SeoInfoScoresResponse(Stage4Description, null),
                SelectedSeoInfos = new SeoInfosResponse(Stage5Description, null),
                SelectedSeoInfo = new SeoInfoResponse(Stage6Description, null)
            };
        }

        // Filtering is there seo.
        var filteredSeoInfoCanBeFound = seoInfosFromCompositeResolver.FilterSeoInfoCanBeFound(storeId, StoreDefaultLanguage, languageCode).ToArray();

        // Select SeoInfo, ObjectTypePriority, Score.
        var seoInfoScores = filteredSeoInfoCanBeFound.SelectSeoInfoScores(storeId, StoreDefaultLanguage, languageCode).ToArray();

        // Filter score greater than 0.
        var filteredSeoInfoScores = seoInfoScores.FilterSeoInfoScoresGreaterThenZero().ToArray();

        // Order by score, then order by desc ObjectTypePriority.
        var orderedSeoInfoScores = filteredSeoInfoScores.OrderSeoInfoScores().ToArray();

        // Select SeoInfos.
        var seoInfos = orderedSeoInfoScores.SelectSeoInfos().ToArray();

        // Select first or default SeoInfo.
        var seoInfo = seoInfos.FirstOrDefault();

        var processOrder = new ProcessOrderSeoInfoResponse
        {
            FoundSeoInfos = new SeoInfosResponse(Stage0Description, seoInfosFromCompositeResolver.ToArray()),
            FilteredSeoInfos = new SeoInfosResponse(Stage1Description, filteredSeoInfoCanBeFound),
            SelectedSeoInfoScores = new SeoInfoScoresResponse(Stage2Description, seoInfoScores),
            FilteredSeoInfoScores = new SeoInfoScoresResponse(Stage3Description, filteredSeoInfoScores),
            OrderedSeoInfoScores = new SeoInfoScoresResponse(Stage4Description, orderedSeoInfoScores),
            SelectedSeoInfos = new SeoInfosResponse(Stage5Description, seoInfos),
            SelectedSeoInfo = new SeoInfoResponse(Stage6Description, seoInfo)
        };

        return processOrder;
    }
}
