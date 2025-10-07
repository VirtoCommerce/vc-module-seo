using System.Collections.Generic;
using VirtoCommerce.Seo.Core.Extensions;
using VirtoCommerce.Seo.Core.Models;
using Xunit;

namespace VirtoCommerce.Seo.Tests;

public class SeoExplainTests
{
    [Fact]
    public void GetBestMatchingSeoInfo_WithExplainTrue_ReturnsStagesAndSeoInfo()
    {
        // Arrange
        var storeId = "store-1";
        var storeDefaultLanguage = "en-US";
        var language = "en-US";
        var categorySeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = language, SemanticUrl = "s1", ObjectType = "Category", IsActive = true };
        var items = new List<SeoInfo> { categorySeoInfo };

        // Act
        var (seoInfo, explainResults) = items.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, language, explain: true);

        // Assert
        Assert.NotNull(explainResults);
        Assert.Equal(6, explainResults.Count);
        Assert.NotNull(seoInfo);
        Assert.Equal(categorySeoInfo.SemanticUrl, seoInfo.SemanticUrl);
    }

    [Fact]
    public void GetBestMatchingSeoInfo_WithExplainFalse_DoesNotReturnStagesButReturnsSeoInfo()
    {
        // Arrange
        var storeId = "store-1";
        var storeDefaultLanguage = "en-US";
        var language = "en-US";
        var categorySeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = language, SemanticUrl = "s1", ObjectType = "Category", IsActive = true };
        var items = new List<SeoInfo> { categorySeoInfo };

        // Act
        var (seoInfo, explainResults) = items.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, language, explain: false);

        // Assert
        Assert.Null(explainResults);
        Assert.NotNull(seoInfo);
        Assert.Equal(categorySeoInfo.SemanticUrl, seoInfo.SemanticUrl);
    }

    [Fact]
    public void GetBestMatchingSeoInfo_NullEnumerable_ReturnsEmptyExplainResultsAndNullSeoInfo()
    {
        // Arrange
        List<SeoInfo> items = null;

        // Act
        var (seoInfo, explainResults) = items.GetBestMatchingSeoInfo("store", "en-US", "en-US", explain: true);

        // Assert
        Assert.NotNull(explainResults);
        Assert.Empty(explainResults);
        Assert.Null(seoInfo);
    }

    [Fact]
    public void GetBestMatchingSeoInfo_InactiveEntriesAreIgnored()
    {
        // Arrange
        var storeId = "store-1";
        var storeDefaultLanguage = "en-US";
        var language = "en-US";
        var inactiveSeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = language, SemanticUrl = "inactive", ObjectType = "Pages", IsActive = false };
        var activeSeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = language, SemanticUrl = "active", ObjectType = "Pages", IsActive = true };
        var items = new List<SeoInfo> { inactiveSeoInfo, activeSeoInfo };

        // Act
        var (seoInfo, explainResults) = items.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, language, explain: true);

        // Assert
        Assert.NotNull(explainResults);
        Assert.NotNull(seoInfo);
        Assert.Equal(activeSeoInfo.SemanticUrl, seoInfo.SemanticUrl);
    }

    [Fact]
    public void GetBestMatchingSeoInfo_StoreSpecificWinsOverGlobal()
    {
        // Arrange
        var storeId = "store-1";
        var storeDefaultLanguage = "en-US";
        var language = "en-US";
        var globalSeoInfo = new SeoInfo { StoreId = null, LanguageCode = language, SemanticUrl = "global", ObjectType = "Pages", IsActive = true };
        var storeSpecificSeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = language, SemanticUrl = "store-specific", ObjectType = "Pages", IsActive = true };
        var items = new List<SeoInfo> { globalSeoInfo, storeSpecificSeoInfo };

        // Act
        var (seoInfo, _) = items.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, language, explain: false);

        // Assert
        Assert.NotNull(seoInfo);
        Assert.Equal(storeSpecificSeoInfo.SemanticUrl, seoInfo.SemanticUrl);
    }
}
