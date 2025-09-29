using System.Collections.Generic;
using VirtoCommerce.Seo.Core.Models.SlugInfo;
using VirtoCommerce.Seo.Core.Extensions;
using VirtoCommerce.Seo.Core.Models;
using Xunit;
using System.Linq;

namespace VirtoCommerce.Seo.Tests
{
    public class SeoExtensionsTests
    {
        [Fact]
        public void GetBestMatchingSeoInfo_WithNullParameters_ReturnsNull()
        {
            // Arrange
            var seoInfos = new List<SeoInfo>
            {
                new() { StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "product1" },
                new() { StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "product2" },
                new() { StoreId = "Store2", LanguageCode = "en-US", SemanticUrl = "product1" },
                new() { StoreId = "Store2", LanguageCode = "en-US", SemanticUrl = "product2" },
            };

            // Act
            var result = seoInfos.GetBestMatchingSeoInfo(storeId: null, storeDefaultLanguage: null, language: null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetBestMatchingSeoInfo_WithNullSlug_ReturnsSeoInfo()
        {
            // Arrange
            var storeId = "Store1";
            var storeDefaultLanguage = "en-US";

            var seoInfos = new List<SeoInfo>
            {
                new() { StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "product1" },
                new() { StoreId = "Store1", LanguageCode = "de-DE", SemanticUrl = "product1" },
                new() { StoreId = "Store2", LanguageCode = "en-US", SemanticUrl = "product1" },
                new() { StoreId = "Store2", LanguageCode = "de-DE", SemanticUrl = "product1" },
            };

            // Act
            var result = seoInfos.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, language: null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Store1", result.StoreId);
            Assert.Equal("en-US", result.LanguageCode);
            Assert.Equal("product1", result.SemanticUrl);
        }

        [Fact]
        public void GetBestMatchingSeoInfo_WithNullLanguage_ReturnsSeoInfo()
        {
            var storeId = "Store1";
            var storeDefaultLanguage = "en-US";

            // Arrange
            var seoInfos = new List<SeoInfo>
            {
                new() { StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "product1" },
                new() { StoreId = "Store1", LanguageCode = "de-DE", SemanticUrl = "product1" },
                new() { StoreId = "Store2", LanguageCode = "en-US", SemanticUrl = "product1" },
                new() { StoreId = "Store2", LanguageCode = "de-DE", SemanticUrl = "product1" },
            };

            // Act
            var result = seoInfos.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, language: null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Store1", result.StoreId);
            Assert.Equal("en-US", result.LanguageCode);
            Assert.Equal("product1", result.SemanticUrl);
        }


        [Fact]
        public void GetBestMatchingSeoInfo_Brand_vs_Category()
        {
            // Arrange
            var storeId = "B2B-store";
            var storeDefaultLanguage = "en-US";

            var seoInfos = new List<SeoInfo>
            {
                new()
                {
                    SemanticUrl = "absolut",
                    PageTitle = "Absolut",
                    StoreId = "B2B-store",
                    ObjectType = "Category",
                    IsActive = true,
                    LanguageCode = "en-US",
                },
                new()
                {
                    SemanticUrl = "Brands/absolut",
                    StoreId = "B2B-store",
                    ObjectType = "Brand",
                    IsActive = true,
                    LanguageCode = "en-US",
                }
            };

            // Act
            var result = seoInfos.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, language: "en-US");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("B2B-store", result.StoreId);
            Assert.Equal("en-US", result.LanguageCode);
            Assert.Equal("Brands/absolut", result.SemanticUrl);
        }

        [Fact]
        public void GetBestMatchingSeoInfo_WithValidParameters_ReturnsSeoInfo()
        {
            // Arrange
            var storeId = "Store1";
            var storeDefaultLanguage = "en-US";

            var seoInfos = new List<SeoInfo>
            {
                new() { StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "product1" },
                new() { StoreId = "Store1", LanguageCode = "de-DE", SemanticUrl = "product1" },
                new() { StoreId = "Store2", LanguageCode = "en-US", SemanticUrl = "product1" },
                new() { StoreId = "Store2", LanguageCode = "de-DE", SemanticUrl = "product1" },
            };

            // Act
            var result = seoInfos.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, language: "en-US");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Store1", result.StoreId);
            Assert.Equal("en-US", result.LanguageCode);
            Assert.Equal("product1", result.SemanticUrl);
        }

        [Fact]
        public void GetBestMatchingSeoInfo_WithUnknownLanguage_ReturnsSeoInfoWithDefaultStoreLanguage()
        {
            // Arrange
            var storeId = "Store1";
            var storeDefaultLanguage = "en-US";

            var seoInfos = new List<SeoInfo>
            {
                new() { StoreId = "Store1", LanguageCode = null, SemanticUrl = "product1" },
                new() { StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "product1" },
                new() { StoreId = "Store1", LanguageCode = "fr-FR", SemanticUrl = "product1" },
            };

            // Act
            var result = seoInfos.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, language: "de-DE");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result.LanguageCode, storeDefaultLanguage);
        }

        [Fact]
        public void GetBestMatchingSeoInfo_WithUnknownLanguage_ReturnsSeoInfoWithEmptyLanguage()
        {
            // Arrange
            var storeId = "Store1";
            var storeDefaultLanguage = "en-US";

            var seoInfos = new List<SeoInfo>
            {
                new() { StoreId = "Store1", LanguageCode = "fr-FR", SemanticUrl = "product1" },
                new() { StoreId = "Store1", LanguageCode = null, SemanticUrl = "product1" },
            };

            // Act
            var result = seoInfos.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, language: "de-DE");

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.LanguageCode);
        }

        [Fact]
        public void GetBestMatchingSeoInfo_WithObjectType_ReturnsSeoInfoWithHighPriority()
        {
            // Arrange
            var storeId = "Store1";
            var storeDefaultLanguage = "en-US";

            SeoExtensions.OrderedObjectTypes = new List<string> { "Categories", "Pages" };

            var seoInfos = new List<SeoInfo>
            {
                new() { StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "product1", ObjectType = "Category"},
                new() { StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "product1", ObjectType = "Pages"},
            };

            // Act
            var result = seoInfos.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, language: "en-US");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Pages", result.ObjectType);
        }

        [Fact]
        public void GetBestMatchingSeoInfo_NonEqualStore_ReturnsNull()
        {
            // Arrange
            var storeId = "Store2";
            var storeDefaultLanguage = "en-US";

            var seoInfos = new List<SeoInfo>
            {
                new() { StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "product1", ObjectType = "Category"},
                new() { StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "product1", ObjectType = "Pages"},
            };

            // Act
            var result = seoInfos.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, language: "en-US");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetBestMatchingSeoInfo_NonEqualLanguage_ReturnsNull()
        {
            // Arrange
            var storeId = "Store1";
            var storeDefaultLanguage = "en-US";

            var seoInfos = new List<SeoInfo>
            {
                new() { StoreId = "Store1", LanguageCode = "de-DE", SemanticUrl = "product1", ObjectType = "Category"},
                new() { StoreId = "Store1", LanguageCode = "de-DE", SemanticUrl = "product1", ObjectType = "Pages"},
            };

            // Act
            var result = seoInfos.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, language: "en-US");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetBestMatchingSeoInfo_WithEmptyStore_ReturnsValue()
        {
            // Arrange
            var storeId = "Store1";
            var storeDefaultLanguage = "en-US";

            var seoInfos = new List<SeoInfo>
            {
                new() { StoreId = null, LanguageCode = "en-US", SemanticUrl = "product1", ObjectType = "Category"},
                new() { StoreId = null, LanguageCode = "en-US", SemanticUrl = "product1", ObjectType = "Pages"},
            };

            // Act
            var result = seoInfos.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, language: "en-US");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetBestMatchingSeoInfo_WithCorrectParameters_ReturnsActive()
        {
            // Arrange
            var storeId = "Store1";
            var storeDefaultLanguage = "en-US";

            var seoInfos = new List<SeoInfo>
            {
                new() { StoreId = null, LanguageCode = "en-US", SemanticUrl = "product1", IsActive = false },
                new() { StoreId = null, LanguageCode = "en-US", SemanticUrl = "product1"},
            };

            // Act
            var result = seoInfos.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, language: "en-US");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsActive);
        }

        [Fact]
        public void GetBestMatchingSeoInfo_SemanticUrl_IsHigherStore()
        {
            // Arrange
            var storeId = "Store2";
            var storeDefaultLanguage = "en-US";

            var seoInfos = new List<SeoInfo>
            {
                new() { StoreId = "Store2", LanguageCode = "en-US", SemanticUrl = "category/product2"},
                new() { StoreId = null, LanguageCode = "en-US", SemanticUrl = "category/product2"},
            };

            // Act
            var result = seoInfos.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, language: null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Store2", result.StoreId);
        }

        [Fact]
        public void GetSeoInfosResponses_ContainsAllPipelineStages()
        {
            // Arrange
            var storeId = "Store1";
            var storeDefaultLanguage = "en-US";
            var language = "en-US";

            var seoInfos = new List<SeoInfo>
            {
                new() { StoreId = storeId, LanguageCode = language, SemanticUrl = "product1" }
            };

            // Act
            var stages = seoInfos.GetSeoInfosResponses(storeId, storeDefaultLanguage, language);

            // Assert
            Assert.NotNull(stages);
            Assert.Equal(6, stages.Count);
            Assert.Equal(PipelineStage.Original, stages[0].Stage);
            Assert.Equal(PipelineStage.Filtered, stages[1].Stage);
            Assert.Equal(PipelineStage.Scored, stages[2].Stage);
            Assert.Equal(PipelineStage.FilteredScore, stages[3].Stage);
            Assert.Equal(PipelineStage.Ordered, stages[4].Stage);
            Assert.Equal(PipelineStage.Final, stages[5].Stage);
        }

        [Fact]
        public void ObjectTypePriority_InfluencesOrdering()
        {
            var storeId = "Store1";
            var storeDefaultLanguage = "en-US";
            var language = "en-US";

            var cat = new SeoInfo { StoreId = storeId, LanguageCode = language, SemanticUrl = "cat", ObjectType = "Category", IsActive = true };
            var page = new SeoInfo { StoreId = storeId, LanguageCode = language, SemanticUrl = "page", ObjectType = "Pages", IsActive = true };
            var prod = new SeoInfo { StoreId = storeId, LanguageCode = language, SemanticUrl = "prod", ObjectType = "CatalogProduct", IsActive = true };

            var items = new List<SeoInfo> { cat, page, prod };

            var original = SeoExtensions.OrderedObjectTypes.ToList();
            try
            {
                SeoExtensions.OrderedObjectTypes = new List<string> { "Category", "Pages", "CatalogProduct" };

                var stages = items.GetSeoInfosResponses(storeId, storeDefaultLanguage, language);
                var orderedStage = stages.FirstOrDefault(s => s.Stage == PipelineStage.Ordered);
                Assert.NotNull(orderedStage);
                var top = orderedStage.SeoInfoResponses.First();
                Assert.Equal("CatalogProduct", top.SeoInfo.ObjectType);
            }
            finally
            {
                SeoExtensions.OrderedObjectTypes = original;
            }
        }

        [Fact]
        public void ActiveFlag_AffectsSelection()
        {
            var storeId = "Store1";
            var storeDefaultLanguage = "en-US";
            var language = "en-US";

            var inactive = new SeoInfo { StoreId = storeId, LanguageCode = language, SemanticUrl = "p", IsActive = false, ObjectType = "Pages" };
            var active = new SeoInfo { StoreId = storeId, LanguageCode = language, SemanticUrl = "p", IsActive = true, ObjectType = "Pages" };

            var items = new List<SeoInfo> { inactive, active };
            var stages = items.GetSeoInfosResponses(storeId, storeDefaultLanguage, language);
            var final = stages.FirstOrDefault(s => s.Stage == PipelineStage.Final);
            Assert.NotNull(final);
            var chosen = final.SeoInfoResponses.First().SeoInfo;
            Assert.True(chosen.IsActive);
        }

        [Fact]
        public void LanguageFallback_PrefersStoreDefaultOverEmpty()
        {
            var storeId = "Store1";
            var storeDefaultLanguage = "en-US";
            var requestLanguage = "de-DE";

            var english = new SeoInfo { StoreId = storeId, LanguageCode = "en-US", SemanticUrl = "en", ObjectType = "Pages", IsActive = true };
            var emptyLang = new SeoInfo { StoreId = storeId, LanguageCode = null, SemanticUrl = "empty", ObjectType = "Pages", IsActive = true };

            var items = new List<SeoInfo> { emptyLang, english };
            var stages = items.GetSeoInfosResponses(storeId, storeDefaultLanguage, requestLanguage);
            var final = stages.FirstOrDefault(s => s.Stage == PipelineStage.Final);
            var chosen = final.SeoInfoResponses.First().SeoInfo;
            Assert.Equal("en-US", chosen.LanguageCode);
        }

        [Fact]
        public void PriorityMap_IsRebuiltOnOrderedObjectTypesChange()
        {
            var original = SeoExtensions.OrderedObjectTypes.ToList();
            try
            {
                SeoExtensions.OrderedObjectTypes = new List<string> { "A", "B", "C" };
                // create entries with types A,B,C and assert ordering reflects new priority
                var storeId = "Store1";
                var lang = "en-US";
                var a = new SeoInfo { StoreId = storeId, LanguageCode = lang, ObjectType = "A", IsActive = true };
                var b = new SeoInfo { StoreId = storeId, LanguageCode = lang, ObjectType = "B", IsActive = true };
                var c = new SeoInfo { StoreId = storeId, LanguageCode = lang, ObjectType = "C", IsActive = true };

                var items = new List<SeoInfo> { a, b, c };
                var stages = items.GetSeoInfosResponses(storeId, lang, lang);
                var ordered = stages.First(s => s.Stage == PipelineStage.Ordered);
                var top = ordered.SeoInfoResponses.First().SeoInfo;
                Assert.Equal("C", top.ObjectType);
            }
            finally
            {
                SeoExtensions.OrderedObjectTypes = original;
            }
        }
    }
}
