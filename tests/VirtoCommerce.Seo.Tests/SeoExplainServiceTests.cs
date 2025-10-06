using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Seo.Core.Extensions;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Models.Explain;
using VirtoCommerce.Seo.Core.Services;
using VirtoCommerce.Seo.Data.Services;
using Xunit;

namespace VirtoCommerce.Seo.Tests
{
    public class SeoExplainServiceTests
    {
        [Fact]
        public async Task ExplainAsync_WhenResolverReturnsNull_ReturnsEmptyListAndPassesCriteria()
        {
            var storeId = "store-1";
            var storeDefaultLanguage = "en-US";
            var languageCode = "en-US";
            var permalink = "category/product";
            var fakeResolver = new FakeCompositeSeoResolver(null);
            var service = new SeoExplainService(fakeResolver);

            var result = await service.ExplainAsync(storeId, storeDefaultLanguage, languageCode, permalink);

            Assert.NotNull(result);
            Assert.Empty(result);
            Assert.NotNull(fakeResolver.LastCriteria);
            Assert.Equal(storeId, fakeResolver.LastCriteria.StoreId);
            Assert.Equal(languageCode, fakeResolver.LastCriteria.LanguageCode);
            Assert.Equal(permalink, fakeResolver.LastCriteria.Permalink);
        }

        [Fact]
        public async Task ExplainAsync_WhenResolverReturnsEmptyList_ReturnsEmptyList()
        {
            var storeId = "store-1";
            var storeDefaultLanguage = "en-US";
            var languageCode = "en-US";
            var permalink = "category/product";
            var fakeResolver = new FakeCompositeSeoResolver([]);
            var service = new SeoExplainService(fakeResolver);

            var result = await service.ExplainAsync(storeId, storeDefaultLanguage, languageCode, permalink);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task ExplainAsync_MultipleSeoInfos_BestMatchIsCalculatedCorrectly()
        {
            var storeId = "store-1";
            var storeDefaultLanguage = "en-US";
            var languageCode = "en-US";
            var permalink = "category/product";
            var categorySeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = languageCode, SemanticUrl = permalink, ObjectType = "Category", IsActive = true };
            var globalPageSeoInfo = new SeoInfo { StoreId = null, LanguageCode = languageCode, SemanticUrl = "global-page", ObjectType = "Pages", IsActive = true };
            var productSeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = null, SemanticUrl = "prod1", ObjectType = "CatalogProduct", IsActive = true };
            var brandInactiveSeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = languageCode, SemanticUrl = "brand1", ObjectType = "Brand", IsActive = false };
            var items = new List<SeoInfo> { categorySeoInfo, globalPageSeoInfo, productSeoInfo, brandInactiveSeoInfo };
            var fakeResolver = new FakeCompositeSeoResolver(items);
            var service = new SeoExplainService(fakeResolver);

            var result = await service.ExplainAsync(storeId, storeDefaultLanguage, languageCode, permalink);

            var stage6 = result.First(x => x.Stage == SeoExplainStage.Final);
            Assert.NotNull(stage6);
            var first = stage6.SeoExplainItems.First();
            Assert.NotNull(first.SeoInfo);
            Assert.Equal(categorySeoInfo.SemanticUrl, first.SeoInfo.SemanticUrl);
            Assert.Equal(categorySeoInfo.StoreId, first.SeoInfo.StoreId);
            Assert.Equal(categorySeoInfo.LanguageCode, first.SeoInfo.LanguageCode);
        }

        [Fact]
        public async Task ExplainAsync_OnlyGlobalEntries_ReturnsGlobalInStage6()
        {
            var storeId = "store-1";
            var storeDefaultLanguage = "en-US";
            var languageCode = "en-US";
            var permalink = "category/product";
            var globalSeoInfoEnglish = new SeoInfo { StoreId = null, LanguageCode = languageCode, SemanticUrl = "global1", ObjectType = "Pages", IsActive = true };
            var globalSeoInfoEmptyLanguage = new SeoInfo { StoreId = null, LanguageCode = null, SemanticUrl = "global2", ObjectType = "Category", IsActive = true };
            var items = new List<SeoInfo> { globalSeoInfoEnglish, globalSeoInfoEmptyLanguage };
            var fakeResolver = new FakeCompositeSeoResolver(items);
            var service = new SeoExplainService(fakeResolver);

            var result = await service.ExplainAsync(storeId, storeDefaultLanguage, languageCode, permalink);

            var finalStage = result.First(x => x.Stage == SeoExplainStage.Final);
            Assert.NotNull(finalStage);
            var first = finalStage.SeoExplainItems.First();
            Assert.NotNull(first.SeoInfo);
            Assert.Equal(globalSeoInfoEnglish.SemanticUrl, first.SeoInfo.SemanticUrl);
        }

        [Fact]
        public async Task ExplainAsync_NoMatchingStore_Stage5Empty_Stage6ContainsNull()
        {
            var storeId = "store-2";
            var storeDefaultLanguage = "en-US";
            var languageCode = "en-US";
            var items = new List<SeoInfo>
            {
                new() { StoreId = "store-1", LanguageCode = languageCode, SemanticUrl = "s1", ObjectType = "Category", IsActive = true },
                new() { StoreId = "store-1", LanguageCode = languageCode, SemanticUrl = "s2", ObjectType = "Pages", IsActive = true },
                new() { StoreId = "store-1", LanguageCode = languageCode, SemanticUrl = "s3", ObjectType = "Brand", IsActive = true },
                new() { StoreId = "store-1", LanguageCode = null, SemanticUrl = "s4", ObjectType = "Catalog", IsActive = true }
            };
            var fakeResolver = new FakeCompositeSeoResolver(items);
            var service = new SeoExplainService(fakeResolver);

            var result = await service.ExplainAsync(storeId, storeDefaultLanguage, languageCode, "perm");

            Assert.NotNull(result);
            var stage5 = result.First(x => x.Stage == SeoExplainStage.Ordered);
            var stage6 = result.First(x => x.Stage == SeoExplainStage.Final);
            Assert.NotNull(stage5);
            Assert.NotNull(stage6);
            Assert.Empty(stage5.SeoExplainItems);
            Assert.Empty(stage6.SeoExplainItems);
        }

        [Fact]
        public async Task ExplainAsync_LanguageFallbackToStoreDefault_Works()
        {
            var storeId = "store-1";
            var storeDefaultLanguage = "en-US";
            var requestLanguage = "de-DE";
            var permalink = "category/product";
            var englishSeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = "en-US", SemanticUrl = "en", ObjectType = "Pages", IsActive = true };
            var emptyLanguageSeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = null, SemanticUrl = "empty", ObjectType = "Category", IsActive = true };
            var frenchSeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = "fr-FR", SemanticUrl = "fr", ObjectType = "Brand", IsActive = true };
            var items = new List<SeoInfo> { englishSeoInfo, emptyLanguageSeoInfo, frenchSeoInfo };
            var fakeResolver = new FakeCompositeSeoResolver(items);
            var service = new SeoExplainService(fakeResolver);

            var result = await service.ExplainAsync(storeId, storeDefaultLanguage, requestLanguage, permalink);

            var stage6 = result.First(x => x.Stage == SeoExplainStage.Final);
            Assert.NotNull(stage6);
            var chosen = stage6.SeoExplainItems.First().SeoInfo;
            Assert.Equal("en-US", chosen.LanguageCode);
        }

        [Fact]
        public async Task ExplainAsync_LanguageFallbackToNeutral_Works()
        {
            var storeId = "store-1";
            var storeDefaultLanguage = "en-US";
            var requestLanguage = "de-DE";
            var permalink = "category/product";
            var neutralLanguageSeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = null, SemanticUrl = "neutral", ObjectType = "Category", IsActive = true };
            var frenchSeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = "fr-FR", SemanticUrl = "fr", ObjectType = "Brand", IsActive = true };
            var items = new List<SeoInfo> { neutralLanguageSeoInfo, frenchSeoInfo };
            var fakeResolver = new FakeCompositeSeoResolver(items);
            var service = new SeoExplainService(fakeResolver);

            var result = await service.ExplainAsync(storeId, storeDefaultLanguage, requestLanguage, permalink);

            var stage6 = result.First(x => x.Stage == SeoExplainStage.Final);
            Assert.NotNull(stage6);
            var chosen = stage6.SeoExplainItems.First().SeoInfo;
            Assert.Null(chosen.LanguageCode);
            Assert.Equal("neutral", chosen.SemanticUrl);
        }

        [Fact]
        public async Task ExplainAsync_PermalinkMatching_IsCaseInsensitive()
        {
            var storeId = "store-1";
            var storeDefaultLanguage = "en-US";
            var languageCode = "en-US";
            var permalink = "Category/Product";
            var seoInfo = new SeoInfo { StoreId = storeId, LanguageCode = languageCode, SemanticUrl = "category/product", ObjectType = "Category", IsActive = true };
            var items = new List<SeoInfo> { seoInfo };
            var fakeResolver = new FakeCompositeSeoResolver(items);
            var service = new SeoExplainService(fakeResolver);

            var result = await service.ExplainAsync(storeId, storeDefaultLanguage, languageCode, permalink);

            var stage6 = result.First(x => x.Stage == SeoExplainStage.Final);
            Assert.NotNull(stage6);
            var chosen = stage6.SeoExplainItems.First().SeoInfo;
            Assert.NotNull(chosen);
            Assert.Equal(seoInfo.SemanticUrl, chosen.SemanticUrl);
        }

        [Fact]
        public async Task ExplainAsync_LanguageCodeMatching_IsCaseInsensitive()
        {
            var storeId = "store-1";
            var storeDefaultLanguage = "en-US";
            var requestLanguage = "en-us";
            var permalink = "category/product";
            var englishSeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = "en-US", SemanticUrl = "en", ObjectType = "Pages", IsActive = true };
            var items = new List<SeoInfo> { englishSeoInfo };
            var fakeResolver = new FakeCompositeSeoResolver(items);
            var service = new SeoExplainService(fakeResolver);

            var result = await service.ExplainAsync(storeId, storeDefaultLanguage, requestLanguage, permalink);

            var stage6 = result.First(x => x.Stage == SeoExplainStage.Final);
            Assert.NotNull(stage6);
            var chosen = stage6.SeoExplainItems.First().SeoInfo;
            Assert.NotNull(chosen);
            Assert.Equal("en-US", chosen.LanguageCode);
        }

        [Fact]
        public void ExplainAsync_ObjectTypePriorityAffectsOrdering()
        {
            var storeId = "store-1";
            var storeDefaultLanguage = "en-US";
            var languageCode = "en-US";
            var categorySeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = languageCode, SemanticUrl = "category-slug", ObjectType = "Category", IsActive = true };
            var pageSeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = languageCode, SemanticUrl = "page-slug", ObjectType = "Pages", IsActive = true };
            var productSeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = languageCode, SemanticUrl = "product-slug", ObjectType = "CatalogProduct", IsActive = true };
            var items = new List<SeoInfo> { categorySeoInfo, pageSeoInfo, productSeoInfo };
            var original = SeoExtensions.OrderedObjectTypes;
            SeoExtensions.OrderedObjectTypes = ["Category", "Pages", "CatalogProduct"];
            try
            {
                var (_, explainResults) = items.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, languageCode, explain: true);
                var stage5 = explainResults.First(x => x.Stage == SeoExplainStage.Ordered);
                Assert.NotNull(stage5);
                var top = stage5.SeoExplainItems.First();
                Assert.Equal("CatalogProduct", top.SeoInfo.ObjectType);
            }
            finally
            {
                SeoExtensions.OrderedObjectTypes = original;
            }
        }

        [Fact]
        public async Task ExplainAsync_WhenStoreDefaultLanguageIsNull_ResultIsEmpty()
        {
            var storeId = "store-1";
            const string storeDefaultLanguage = null;
            var languageCode = "en-US";
            var permalink = "category/product";
            var seoInfo = new SeoInfo { StoreId = storeId, LanguageCode = languageCode, SemanticUrl = permalink };
            var fakeResolver = new FakeCompositeSeoResolver([seoInfo]);
            var service = new SeoExplainService(fakeResolver);

            var result = await service.ExplainAsync(storeId, storeDefaultLanguage, languageCode, permalink);

            Assert.Empty(result);
        }

        [Fact]
        public async Task ExplainAsync_WhenStoreIdIsNull_ResultIsEmpty()
        {
            const string storeId = null;
            var storeDefaultLanguage = "en-US";
            var languageCode = "en-US";
            var permalink = "category/product";
            var seoInfo = new SeoInfo { StoreId = storeId, LanguageCode = languageCode, SemanticUrl = permalink };
            var fakeResolver = new FakeCompositeSeoResolver([seoInfo]);
            var service = new SeoExplainService(fakeResolver);

            var result = await service.ExplainAsync(storeId, storeDefaultLanguage, languageCode, permalink);

            Assert.Empty(result);
        }

        [Fact]
        public async Task ExplainAsync_WhenResolverThrows_PropagatesException()
        {
            var storeId = "store-1";
            var storeDefaultLanguage = "en-US";
            var languageCode = "en-US";
            var permalink = "category/product";
            var throwing = new ThrowingResolver();
            var service = new SeoExplainService(throwing);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.ExplainAsync(storeId, storeDefaultLanguage, languageCode, permalink));
        }

        [Fact]
        public void ExplainBestMatchingSeoInfo_WithExplainTrue_ReturnsStagesAndSeoInfo()
        {
            var storeId = "store-1";
            var storeDefaultLanguage = "en-US";
            var language = "en-US";
            var categorySeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = language, SemanticUrl = "s1", ObjectType = "Category", IsActive = true };
            var items = new List<SeoInfo> { categorySeoInfo };

            var (seoInfo, explainResults) = items.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, language, explain: true);

            Assert.NotNull(explainResults);
            Assert.Equal(6, explainResults.Count);
            Assert.NotNull(seoInfo);
            Assert.Equal(categorySeoInfo.SemanticUrl, seoInfo.SemanticUrl);
        }

        [Fact]
        public void ExplainBestMatchingSeoInfo_WithExplainFalse_DoesNotReturnStagesButReturnsSeoInfo()
        {
            var storeId = "store-1";
            var storeDefaultLanguage = "en-US";
            var language = "en-US";
            var categorySeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = language, SemanticUrl = "s1", ObjectType = "Category", IsActive = true };
            var items = new List<SeoInfo> { categorySeoInfo };

            var (seoInfo, explainResults) = items.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, language, explain: false);

            Assert.Null(explainResults);
            Assert.NotNull(seoInfo);
            Assert.Equal(categorySeoInfo.SemanticUrl, seoInfo.SemanticUrl);
        }

        [Fact]
        public void ExplainBestMatchingSeoInfo_NullEnumerable_ReturnsNulls()
        {
            List<SeoInfo> items = null;

            var (seoInfo, explainResults) = items.GetBestMatchingSeoInfo("store", "en-US", "en-US", explain: true);

            Assert.Null(explainResults);
            Assert.Null(seoInfo);
        }

        [Fact]
        public void ExplainBestMatchingSeoInfo_InactiveEntriesAreIgnored()
        {
            var storeId = "store-1";
            var storeDefaultLanguage = "en-US";
            var language = "en-US";
            var inactiveSeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = language, SemanticUrl = "inactive", ObjectType = "Pages", IsActive = false };
            var activeSeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = language, SemanticUrl = "active", ObjectType = "Pages", IsActive = true };
            var items = new List<SeoInfo> { inactiveSeoInfo, activeSeoInfo };

            var (seoInfo, explainResults) = items.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, language, explain: true);

            Assert.NotNull(explainResults);
            Assert.NotNull(seoInfo);
            Assert.Equal(activeSeoInfo.SemanticUrl, seoInfo.SemanticUrl);
        }

        [Fact]
        public void ExplainBestMatchingSeoInfo_StoreSpecificWinsOverGlobal()
        {
            var storeId = "store-1";
            var storeDefaultLanguage = "en-US";
            var language = "en-US";
            var globalSeoInfo = new SeoInfo { StoreId = null, LanguageCode = language, SemanticUrl = "global", ObjectType = "Pages", IsActive = true };
            var storeSpecificSeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = language, SemanticUrl = "store-specific", ObjectType = "Pages", IsActive = true };
            var items = new List<SeoInfo> { globalSeoInfo, storeSpecificSeoInfo };

            var (seoInfo, _) = items.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, language, explain: false);

            Assert.NotNull(seoInfo);
            Assert.Equal(storeSpecificSeoInfo.SemanticUrl, seoInfo.SemanticUrl);
        }

        [Fact]
        public void ExplainBestMatchingSeoInfo_TieBreakByObjectTypePriority()
        {
            var storeId = "store-1";
            var storeDefaultLanguage = "en-US";
            var language = "en-US";
            var seoInfoA = new SeoInfo { StoreId = storeId, LanguageCode = language, SemanticUrl = "item-a", ObjectType = "Pages", IsActive = true };
            var seoInfoB = new SeoInfo { StoreId = storeId, LanguageCode = language, SemanticUrl = "item-b", ObjectType = "CatalogProduct", IsActive = true };
            var items = new List<SeoInfo> { seoInfoA, seoInfoB };
            var original = SeoExtensions.OrderedObjectTypes;
            SeoExtensions.OrderedObjectTypes = ["Pages", "CatalogProduct"];
            try
            {
                var (seoInfo, _) = items.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, language, explain: true);

                Assert.NotNull(seoInfo);
                Assert.Equal("item-b", seoInfo.SemanticUrl);
            }
            finally
            {
                SeoExtensions.OrderedObjectTypes = original;
            }
        }

        private sealed class FakeCompositeSeoResolver(IList<SeoInfo> toReturn) : ICompositeSeoResolver
        {
            public SeoSearchCriteria LastCriteria { get; private set; }

            public Task<IList<SeoInfo>> FindSeoAsync(SeoSearchCriteria criteria)
            {
                LastCriteria = criteria;
                return Task.FromResult(toReturn);
            }
        }

        private sealed class ThrowingResolver : ICompositeSeoResolver
        {
            public Task<IList<SeoInfo>> FindSeoAsync(SeoSearchCriteria criteria) => throw new InvalidOperationException("fail");
        }
    }
}
