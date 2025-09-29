using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Models.SlugInfo;
using VirtoCommerce.Seo.Core.Services;
using VirtoCommerce.Seo.Data.Services;
using VirtoCommerce.Seo.Core.Extensions;
using Xunit;

namespace VirtoCommerce.Seo.Tests
{
    public class SlugInfoServiceTests
    {
        private sealed class FakeCompositeSeoResolver : ICompositeSeoResolver
        {
            private readonly IList<SeoInfo> _toReturn;
            public SeoSearchCriteria LastCriteria { get; private set; }

            public FakeCompositeSeoResolver(IList<SeoInfo> toReturn)
            {
                _toReturn = toReturn;
            }

            public Task<IList<SeoInfo>> FindSeoAsync(SeoSearchCriteria criteria)
            {
                LastCriteria = criteria;
                return Task.FromResult(_toReturn);
            }
        }

        private sealed class ThrowingResolver : ICompositeSeoResolver
        {
            public Task<IList<SeoInfo>> FindSeoAsync(SeoSearchCriteria criteria) => throw new System.InvalidOperationException("fail");
        }

        [Fact]
        public async Task GetExplainAsync_WhenResolverReturnsNull_ReturnsEmptyResultsAndPassesCriteria()
        {
            // Arrange
            var storeId = "store-1";
            var storeDefaultLanguage = "en-US";
            var languageCode = "en-US";
            var permalink = "category/product";

            var fakeResolver = new FakeCompositeSeoResolver(null);
            var service = new SlugInfoService(fakeResolver);

            // Act
            var result = await service.GetExplainAsync(storeId, storeDefaultLanguage, languageCode, permalink);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(storeId, result.StoreId);
            Assert.Equal(languageCode, result.LanguageCode);
            Assert.Equal(permalink, result.Permalink);
            Assert.NotNull(result.Results);
            Assert.Empty(result.Results);

            Assert.NotNull(fakeResolver.LastCriteria);
            Assert.Equal(storeId, fakeResolver.LastCriteria.StoreId);
            Assert.Equal(languageCode, fakeResolver.LastCriteria.LanguageCode);
            Assert.Equal(permalink, fakeResolver.LastCriteria.Permalink);
        }

        [Fact]
        public async Task GetExplainAsync_WhenResolverReturnsEmptyList_ReturnsEmptyResults()
        {
            // Arrange
            var storeId = "store-1";
            var storeDefaultLanguage = "en-US";
            var languageCode = "en-US";
            var permalink = "category/product";

            var fakeResolver = new FakeCompositeSeoResolver(new List<SeoInfo>());
            var service = new SlugInfoService(fakeResolver);

            // Act
            var result = await service.GetExplainAsync(storeId, storeDefaultLanguage, languageCode, permalink);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Results);
            Assert.Empty(result.Results);
        }

        [Fact]
        public async Task GetExplainAsync_MultipleSeoInfos_BestMatchIsCalculatedCorrectly()
        {
            // Arrange
            var storeId = "store-1";
            var storeDefaultLanguage = "en-US";
            var languageCode = "en-US";
            var permalink = "category/product";

            var a = new SeoInfo { StoreId = storeId, LanguageCode = languageCode, SemanticUrl = permalink, ObjectType = "Category", IsActive = true };
            var b = new SeoInfo { StoreId = null, LanguageCode = languageCode, SemanticUrl = "global-page", ObjectType = "Pages", IsActive = true };
            var c = new SeoInfo { StoreId = storeId, LanguageCode = null, SemanticUrl = "prod1", ObjectType = "CatalogProduct", IsActive = true };
            var d = new SeoInfo { StoreId = storeId, LanguageCode = languageCode, SemanticUrl = "brand1", ObjectType = "Brand", IsActive = false };

            var items = new List<SeoInfo> { a, b, c, d };
            var fakeResolver = new FakeCompositeSeoResolver(items);
            var service = new SlugInfoService(fakeResolver);

            // Act
            var result = await service.GetExplainAsync(storeId, storeDefaultLanguage, languageCode, permalink);

            // Assert
            Assert.NotNull(result);
            var stage6 = result.Results.FirstOrDefault(r => r.Description.StartsWith("Stage 6"));
            Assert.NotNull(stage6);
            var first = stage6.SeoInfoResponses.First();
            Assert.NotNull(first.SeoInfo);
            // 'a' should be best matching candidate for store1/en-US
            Assert.Equal(a.SemanticUrl, first.SeoInfo.SemanticUrl);
            Assert.Equal(a.StoreId, first.SeoInfo.StoreId);
            Assert.Equal(a.LanguageCode, first.SeoInfo.LanguageCode);
        }

        [Fact]
        public async Task GetExplainAsync_OnlyGlobalEntries_ReturnsGlobalInStage6()
        {
            // Arrange
            var storeId = "store-1";
            var storeDefaultLanguage = "en-US";
            var languageCode = "en-US";
            var permalink = "category/product";

            var g1 = new SeoInfo { StoreId = null, LanguageCode = languageCode, SemanticUrl = "global1", ObjectType = "Pages", IsActive = true };
            var g2 = new SeoInfo { StoreId = null, LanguageCode = null, SemanticUrl = "global2", ObjectType = "Category", IsActive = true };

            var fakeResolver = new FakeCompositeSeoResolver(new List<SeoInfo> { g1, g2 });
            var service = new SlugInfoService(fakeResolver);

            // Act
            var result = await service.GetExplainAsync(storeId, storeDefaultLanguage, languageCode, permalink);

            // Assert
            var stage6 = result.Results.FirstOrDefault(r => r.Description.StartsWith("Stage 6"));
            Assert.NotNull(stage6);
            var first = stage6.SeoInfoResponses.First();
            Assert.NotNull(first.SeoInfo);
            Assert.Equal(g1.SemanticUrl, first.SeoInfo.SemanticUrl);
        }

        [Fact]
        public async Task GetExplainAsync_NoMatchingStore_Stage5Empty_Stage6ContainsNull()
        {
            // Arrange
            var storeId = "store-2";
            var storeDefaultLanguage = "en-US";
            var languageCode = "en-US";

            // All entries belong to store-1
            var items = new List<SeoInfo>
            {
                new SeoInfo { StoreId = "store-1", LanguageCode = languageCode, SemanticUrl = "s1", ObjectType = "Category", IsActive = true },
                new SeoInfo { StoreId = "store-1", LanguageCode = languageCode, SemanticUrl = "s2", ObjectType = "Pages", IsActive = true },
                new SeoInfo { StoreId = "store-1", LanguageCode = languageCode, SemanticUrl = "s3", ObjectType = "Brand", IsActive = true },
                new SeoInfo { StoreId = "store-1", LanguageCode = null, SemanticUrl = "s4", ObjectType = "Catalog", IsActive = true }
            };

            var fakeResolver = new FakeCompositeSeoResolver(items);
            var service = new SlugInfoService(fakeResolver);

            // Act
            var result = await service.GetExplainAsync(storeId, storeDefaultLanguage, languageCode, "perm");

            // Assert
            // stage 5 should be empty because no item matches store-2
            var stage5 = result.Results.FirstOrDefault(r => r.Description.StartsWith("Stage 5"));
            Assert.NotNull(stage5);
            Assert.Empty(stage5.SeoInfoResponses);

            var stage6 = result.Results.FirstOrDefault(r => r.Description.StartsWith("Stage 6"));
            Assert.NotNull(stage6);
            // stage6 contains no candidates
            Assert.Empty(stage6.SeoInfoResponses);
        }

        [Fact]
        public async Task GetExplainAsync_LanguageFallbackToStoreDefault_Works()
        {
            // Arrange
            var storeId = "store-1";
            var storeDefaultLanguage = "en-US";
            var requestLanguage = "de-DE"; // not present
            var permalink = "category/product";

            var english = new SeoInfo { StoreId = storeId, LanguageCode = "en-US", SemanticUrl = "en", ObjectType = "Pages", IsActive = true };
            var emptyLang = new SeoInfo { StoreId = storeId, LanguageCode = null, SemanticUrl = "empty", ObjectType = "Category", IsActive = true };
            var french = new SeoInfo { StoreId = storeId, LanguageCode = "fr-FR", SemanticUrl = "fr", ObjectType = "Brand", IsActive = true };

            var fakeResolver = new FakeCompositeSeoResolver(new List<SeoInfo> { english, emptyLang, french });
            var service = new SlugInfoService(fakeResolver);

            // Act
            var result = await service.GetExplainAsync(storeId, storeDefaultLanguage, requestLanguage, permalink);

            // Assert
            var stage6 = result.Results.FirstOrDefault(r => r.Description.StartsWith("Stage 6"));
            Assert.NotNull(stage6);
            var chosen = stage6.SeoInfoResponses.First().SeoInfo;
            // Should pick the one with store default language (en-US)
            Assert.Equal("en-US", chosen.LanguageCode);
        }

        [Fact]
        public async Task GetExplainAsync_ObjectTypePriorityAffectsOrdering()
        {
            // Arrange
            var storeId = "store-1";
            var storeDefaultLanguage = "en-US";
            var languageCode = "en-US";
            var permalink = "category/product";

            var cat = new SeoInfo { StoreId = storeId, LanguageCode = languageCode, SemanticUrl = "cat", ObjectType = "Category", IsActive = true };
            var page = new SeoInfo { StoreId = storeId, LanguageCode = languageCode, SemanticUrl = "page", ObjectType = "Pages", IsActive = true };
            var prod = new SeoInfo { StoreId = storeId, LanguageCode = languageCode, SemanticUrl = "prod", ObjectType = "CatalogProduct", IsActive = true };

            var items = new List<SeoInfo> { cat, page, prod };
            var fakeResolver = new FakeCompositeSeoResolver(items);

            // Change priority order for test and restore after
            var original = SeoExtensions.OrderedObjectTypes;
            SeoExtensions.OrderedObjectTypes = new List<string> { "Category", "Pages", "CatalogProduct" };
            try
            {
                var service = new SlugInfoService(fakeResolver);

                // Act
                var result = await service.GetExplainAsync(storeId, storeDefaultLanguage, languageCode, permalink);

                // Assert
                var stage5 = result.Results.FirstOrDefault(r => r.Description.StartsWith("Stage 5"));
                Assert.NotNull(stage5);
                // highest priority should be the last element in OrderedObjectTypes -> CatalogProduct
                var top = stage5.SeoInfoResponses.First();
                Assert.Equal("CatalogProduct", top.SeoInfo.ObjectType);
            }
            finally
            {
                SeoExtensions.OrderedObjectTypes = original;
            }
        }

        [Fact]
        public async Task GetExplainAsync_WhenStoreDefaultLanguageIsNull_ResultResultsIsNull()
        {
            // Arrange
            string storeId = "store-1";
            string storeDefaultLanguage = null;
            string languageCode = "en-US";
            string permalink = "category/product";

            var seoInfo = new SeoInfo { StoreId = storeId, LanguageCode = languageCode, SemanticUrl = permalink };
            var fakeResolver = new FakeCompositeSeoResolver(new List<SeoInfo> { seoInfo });
            var service = new SlugInfoService(fakeResolver);

            // Act
            var result = await service.GetExplainAsync(storeId, storeDefaultLanguage, languageCode, permalink);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Results);
        }

        [Fact]
        public async Task GetExplainAsync_WhenStoreIdIsNull_ResultResultsIsNull()
        {
            // Arrange
            string storeId = null;
            string storeDefaultLanguage = "en-US";
            string languageCode = "en-US";
            string permalink = "category/product";

            var seoInfo = new SeoInfo { StoreId = storeId, LanguageCode = languageCode, SemanticUrl = permalink };
            var fakeResolver = new FakeCompositeSeoResolver(new List<SeoInfo> { seoInfo });
            var service = new SlugInfoService(fakeResolver);

            // Act
            var result = await service.GetExplainAsync(storeId, storeDefaultLanguage, languageCode, permalink);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Results);
        }

        [Fact]
        public async Task GetExplainAsync_WhenResolverThrows_PropagatesException()
        {
            // Arrange
            var storeId = "store-1";
            var storeDefaultLanguage = "en-US";
            var languageCode = "en-US";
            var permalink = "category/product";

            var throwing = new ThrowingResolver();
            var service = new SlugInfoService(throwing);

            // Act / Assert
            await Assert.ThrowsAsync<System.InvalidOperationException>(async () =>
                await service.GetExplainAsync(storeId, storeDefaultLanguage, languageCode, permalink));
        }
    }
}
