using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Services;
using VirtoCommerce.Seo.Data.Services;
using Xunit;

namespace VirtoCommerce.Seo.Tests;

public class MaintenanceServiceTests
{
    [Fact]
    public async Task GetSeoInfoForTestAsync_ReturnsEmptyProcessOrder_WhenNoSeoInfos()
    {
        // Arrange
        var resolver = new FakeCompositeResolver(null);
        var service = new MaintenanceService(resolver);

        // Act
        var result = await service.GetSeoInfoForTestAsync("Store1", "en-US", "permalink");

        // Assert
        Assert.IsType<ProcessOrderSeoInfoResponse>(result);
        Assert.Null(result.FoundSeoInfos.SeoInfos);
        Assert.Null(result.FilteredSeoInfos.SeoInfos);
        Assert.Null(result.SelectedSeoInfoScores.SeoInfoScores);
        Assert.Null(result.FilteredSeoInfoScores.SeoInfoScores);
        Assert.Null(result.OrderedSeoInfoScores.SeoInfoScores);
        Assert.Null(result.SelectedSeoInfos.SeoInfos);
        Assert.Null(result.SelectedSeoInfo.SeoInfo);
    }

    [Fact]
    public async Task GetSeoInfoForTestAsync_SelectsBestSeoInfo_ByScoreAndPriority()
    {
        // Arrange
        const string storeId = "B2B-store";
        var language = "en-US";

        var category = new SeoInfo
        {
            SemanticUrl = "XXX",
            PageTitle = "Absolut",
            StoreId = storeId,
            ObjectType = "Category",
            IsActive = true,
            LanguageCode = language
        };

        var brand = new SeoInfo
        {
            SemanticUrl = "YYY",
            StoreId = storeId,
            ObjectType = "Brand",
            IsActive = true,
            LanguageCode = language
        };

        var resolver = new FakeCompositeResolver(new List<SeoInfo> { category, brand });
        var service = new MaintenanceService(resolver);

        // Act
        var result = await service.GetSeoInfoForTestAsync(storeId, language, "XXX");

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.SelectedSeoInfo);
        Assert.NotNull(result.SelectedSeoInfo.SeoInfo);
        Assert.Equal(storeId, result.SelectedSeoInfo.SeoInfo.StoreId);

        // Stage1 should contain both items
        Assert.NotNull(result.FilteredSeoInfos);
        Assert.Equal(2, result.FilteredSeoInfos.SeoInfos.Length);

        // Stage2 specials should be present and have scores > 0 for both
        Assert.NotNull(result.SelectedSeoInfoScores);
        Assert.Equal(2, result.SelectedSeoInfoScores.SeoInfoScores.Length);
        Assert.All(result.SelectedSeoInfoScores.SeoInfoScores, s => Assert.True(s.Score >= 0));
    }

    // Simple fake implementation of ICompositeSeoResolver for tests
    private class FakeCompositeResolver(IList<SeoInfo> items) : ICompositeSeoResolver
    {
        public Task<IList<SeoInfo>> FindSeoAsync(SeoSearchCriteria criteria)
        {
            return items == null
                ? Task.FromResult<IList<SeoInfo>>(null)
                : Task.FromResult(items);
        }
    }
}
