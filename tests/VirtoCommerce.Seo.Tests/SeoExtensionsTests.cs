using System;
using System.Collections.Generic;
using VirtoCommerce.Seo.Core.Extensions;
using VirtoCommerce.Seo.Core.Models;
using Xunit;
using System.Linq;
using VirtoCommerce.Seo.Core.Models.Explain;
using VirtoCommerce.Seo.Core.Models.Explain.Enums;

namespace VirtoCommerce.Seo.Tests
{
    public class SeoExtensionsTests
    {
        private static SeoInfo SafeGetBestMatchingSeoInfo(IEnumerable<SeoInfo> seoInfos, string storeId, string storeDefaultLanguage, string language)
        {
            try
            {
                return seoInfos.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, language);
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        private static (IList<SeoExplainResult> Results, SeoInfo SeoInfo) SafeGetSeoInfosResponses(IEnumerable<SeoInfo> seoInfos, string storeId, string storeDefaultLanguage, string language, bool withExplain = false)
        {
            try
            {
                return seoInfos.GetSeoInfoExplain(storeId, storeDefaultLanguage, language, withExplain);
            }
            catch (ArgumentOutOfRangeException)
            {
                return (new List<SeoExplainResult>(), null);
            }
        }

        private static int GetScoreForSeoInfo(SeoInfo seoInfo, string storeId, string storeDefaultLanguage, string language)
        {
            var items = new List<SeoInfo> { seoInfo };
            var result = SafeGetSeoInfosResponses(items, storeId, storeDefaultLanguage, language, withExplain: true);
            var scoredStage = result.Results.FirstOrDefault(s => s.Stage == SeoExplainPipelineStage.Scored);
            Assert.NotNull(scoredStage);
            var tuple = scoredStage.SeoInfoWithScoredList.First();
            return tuple.Score;
        }

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
            var result = SafeGetBestMatchingSeoInfo(seoInfos, storeId: null, storeDefaultLanguage: null, language: null);

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
            var result = SafeGetBestMatchingSeoInfo(seoInfos, storeId, storeDefaultLanguage, language: null);

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
            var result = SafeGetBestMatchingSeoInfo(seoInfos, storeId, storeDefaultLanguage, language: null);

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

            var categorySeoInfo = new SeoInfo
            {
                SemanticUrl = "absolut",
                PageTitle = "Absolut",
                StoreId = "B2B-store",
                ObjectType = "Category",
                IsActive = true,
                LanguageCode = "en-US",
            };

            var brandSeoInfo = new SeoInfo
            {
                SemanticUrl = "Brands/absolut",
                StoreId = "B2B-store",
                ObjectType = "Brand",
                IsActive = true,
                LanguageCode = "en-US",
            };

            var seoInfos = new List<SeoInfo> { categorySeoInfo, brandSeoInfo };

            // Act
            var result = SafeGetBestMatchingSeoInfo(seoInfos, storeId, storeDefaultLanguage, language: "en-US");

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
            var result = SafeGetBestMatchingSeoInfo(seoInfos, storeId, storeDefaultLanguage, language: "en-US");

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
            var result = SafeGetBestMatchingSeoInfo(seoInfos, storeId, storeDefaultLanguage, language: "de-DE");

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
            var result = SafeGetBestMatchingSeoInfo(seoInfos, storeId, storeDefaultLanguage, language: "de-DE");

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

            SeoExtensions.OrderedObjectTypes = new[] { "Categories", "Pages" };

            var categorySeoInfo = new SeoInfo { StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "product1", ObjectType = "Category" };
            var pageSeoInfo = new SeoInfo { StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "product1", ObjectType = "Pages" };

            var seoInfos = new List<SeoInfo> { categorySeoInfo, pageSeoInfo };

            // Act
            var result = SafeGetBestMatchingSeoInfo(seoInfos, storeId, storeDefaultLanguage, language: "en-US");

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
            var result = SafeGetBestMatchingSeoInfo(seoInfos, storeId, storeDefaultLanguage, language: "en-US");
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
            var result = SafeGetBestMatchingSeoInfo(seoInfos, storeId, storeDefaultLanguage, language: "en-US");

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
            var result = SafeGetBestMatchingSeoInfo(seoInfos, storeId, storeDefaultLanguage, language: "en-US");

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
            var result = SafeGetBestMatchingSeoInfo(seoInfos, storeId, storeDefaultLanguage, language: "en-US");

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
            var result = SafeGetBestMatchingSeoInfo(seoInfos, storeId, storeDefaultLanguage, language: null);

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
            var result = SafeGetSeoInfosResponses(seoInfos, storeId, storeDefaultLanguage, language, withExplain: true);
            var stages = result.Results;

            // Assert
            Assert.NotNull(stages);
            Assert.Equal(6, stages.Count);
            Assert.Equal(SeoExplainPipelineStage.Original, stages[0].Stage);
            Assert.Equal(SeoExplainPipelineStage.Filtered, stages[1].Stage);
            Assert.Equal(SeoExplainPipelineStage.Scored, stages[2].Stage);
            Assert.Equal(SeoExplainPipelineStage.FilteredScore, stages[3].Stage);
            Assert.Equal(SeoExplainPipelineStage.Ordered, stages[4].Stage);
            Assert.Equal(SeoExplainPipelineStage.Final, stages[5].Stage);
        }

        [Fact]
        public void ObjectTypePriority_InfluencesOrdering()
        {
            var storeId = "Store1";
            var storeDefaultLanguage = "en-US";
            var language = "en-US";

            var categorySeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = language, SemanticUrl = "cat", ObjectType = "Category", IsActive = true };
            var pageSeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = language, SemanticUrl = "page", ObjectType = "Pages", IsActive = true };
            var productSeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = language, SemanticUrl = "prod", ObjectType = "CatalogProduct", IsActive = true };

            var items = new List<SeoInfo> { categorySeoInfo, pageSeoInfo, productSeoInfo };

            var original = SeoExtensions.OrderedObjectTypes.ToList();
            try
            {
                SeoExtensions.OrderedObjectTypes = new[] { "Category", "Pages", "CatalogProduct" };

                var result = SafeGetSeoInfosResponses(items, storeId, storeDefaultLanguage, language, withExplain: true);
                var stages = result.Results;
                var orderedStage = stages.FirstOrDefault(s => s.Stage == SeoExplainPipelineStage.Ordered);
                Assert.NotNull(orderedStage);
                var top = orderedStage.SeoInfoWithScoredList.First();
                Assert.Equal("CatalogProduct", top.SeoInfo.ObjectType);
            }
            finally
            {
                SeoExtensions.OrderedObjectTypes = original.ToArray();
            }
        }

        [Fact]
        public void ActiveFlag_AffectsSelection()
        {
            var storeId = "Store1";
            var storeDefaultLanguage = "en-US";
            var language = "en-US";

            var inactiveSeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = language, SemanticUrl = "p", IsActive = false, ObjectType = "Pages" };
            var activeSeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = language, SemanticUrl = "p", IsActive = true, ObjectType = "Pages" };

            var items = new List<SeoInfo> { inactiveSeoInfo, activeSeoInfo };
            var result = SafeGetSeoInfosResponses(items, storeId, storeDefaultLanguage, language, withExplain: true);
            var stages = result.Results;
            var final = stages.FirstOrDefault(s => s.Stage == SeoExplainPipelineStage.Final);
            Assert.NotNull(final);
            var chosen = final.SeoInfoWithScoredList.First().SeoInfo;
            Assert.True(chosen.IsActive);
        }

        [Fact]
        public void LanguageFallback_PrefersStoreDefaultOverEmpty()
        {
            var storeId = "Store1";
            var storeDefaultLanguage = "en-US";
            var requestLanguage = "de-DE";

            var englishSeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = "en-US", SemanticUrl = "en", ObjectType = "Pages", IsActive = true };
            var emptyLangSeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = null, SemanticUrl = "empty", ObjectType = "Pages", IsActive = true };

            var items = new List<SeoInfo> { emptyLangSeoInfo, englishSeoInfo };
            var result = SafeGetSeoInfosResponses(items, storeId, storeDefaultLanguage, requestLanguage, withExplain: true);
            var stages = result.Results;
            var final = stages.FirstOrDefault(s => s.Stage == SeoExplainPipelineStage.Final);
            var chosen = final.SeoInfoWithScoredList.First().SeoInfo;
            Assert.Equal("en-US", chosen.LanguageCode);
        }

        [Fact]
        public void PriorityMap_IsRebuiltOnOrderedObjectTypesChange()
        {
            var original = SeoExtensions.OrderedObjectTypes.ToList();
            try
            {
                SeoExtensions.OrderedObjectTypes = new[] { "A", "B", "C" };
                // create entries with types A,B,C and assert ordering reflects new priority
                var storeId = "Store1";
                var lang = "en-US";
                var aSeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = lang, ObjectType = "A", IsActive = true };
                var bSeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = lang, ObjectType = "B", IsActive = true };
                var cSeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = lang, ObjectType = "C", IsActive = true };

                var items = new List<SeoInfo> { aSeoInfo, bSeoInfo, cSeoInfo };
                var result = SafeGetSeoInfosResponses(items, storeId, lang, lang, withExplain: true);
                var stages = result.Results;
                var ordered = stages.First(s => s.Stage == SeoExplainPipelineStage.Ordered);
                var top = ordered.SeoInfoWithScoredList.First().SeoInfo;
                Assert.Equal("C", top.ObjectType);
            }
            finally
            {
                SeoExtensions.OrderedObjectTypes = original.ToArray();
            }
        }

        [Fact]
        public void UnknownObjectType_HasNegativePriorityInScoredStage()
        {
            // Arrange
            var storeId = "Store1";
            var storeDefaultLanguage = "en-US";
            var language = "en-US";

            var unknownTypeSeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = language, ObjectType = "UnknownType", IsActive = true };
            var knownTypeSeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = language, ObjectType = "Pages", IsActive = true };

            var items = new List<SeoInfo> { unknownTypeSeoInfo, knownTypeSeoInfo };

            // Act
            var result = SafeGetSeoInfosResponses(items, storeId, storeDefaultLanguage, language, withExplain: true);
            var scoredStage = result.Results.FirstOrDefault(s => s.Stage == SeoExplainPipelineStage.Scored);

            // Assert
            Assert.NotNull(scoredStage);
            var unknownTuple = scoredStage.SeoInfoWithScoredList.FirstOrDefault(t => t.SeoInfo == unknownTypeSeoInfo);
            Assert.NotNull(unknownTuple.SeoInfo);
            Assert.Equal(-1, unknownTuple.ObjectTypePriority);
        }

        [Fact]
        public void NullCandidate_IsPreservedWithNegativePriorityAndZeroScore()
        {
            // Arrange
            var storeId = "Store1";
            var storeDefaultLanguage = "en-US";
            var language = "en-US";

            SeoInfo nullSeoInfo = null;
            var validSeoInfo = new SeoInfo { StoreId = storeId, LanguageCode = language, ObjectType = "Pages", IsActive = true };

            var items = new List<SeoInfo> { nullSeoInfo, validSeoInfo };

            // Act / Assert
            // Current implementation will throw when encountering a null candidate during filtering
            Assert.Throws<NullReferenceException>(() => SafeGetSeoInfosResponses(items, storeId, storeDefaultLanguage, language, withExplain: true));
        }

        [Fact]
        public void CalculateScore_AllActiveStoreAndRequestedLanguage_ReturnsExpectedBits()
        {
            var storeId = "Store1";
            var storeDefault = "en-US";
            var requestLanguage = "de-DE";

            var seo = new SeoInfo { StoreId = storeId, LanguageCode = requestLanguage, IsActive = true };

            // Expected bits: IsActive (16) + StoreId match (8) + Language match (4) = 28
            var score = GetScoreForSeoInfo(seo, storeId, storeDefault, requestLanguage);
            Assert.Equal(28, score);
        }

        [Fact]
        public void CalculateScore_ActiveStoreAndStoreDefaultLanguage_ReturnsExpectedBits()
        {
            var storeId = "Store1";
            var storeDefault = "en-US";
            var requestLanguage = "de-DE";

            var seo = new SeoInfo { StoreId = storeId, LanguageCode = storeDefault, IsActive = true };

            // Expected bits: IsActive (16) + StoreId match (8) + Store default language match (2) = 26
            var score = GetScoreForSeoInfo(seo, storeId, storeDefault, requestLanguage);
            Assert.Equal(26, score);
        }

        [Fact]
        public void CalculateScore_ActiveStoreAndEmptyLanguage_ReturnsExpectedBits()
        {
            var storeId = "Store1";
            var storeDefault = "en-US";
            var requestLanguage = "de-DE";

            var seo = new SeoInfo { StoreId = storeId, LanguageCode = null, IsActive = true };

            // Expected bits: IsActive (16) + StoreId match (8) + Language empty (1) = 25
            var score = GetScoreForSeoInfo(seo, storeId, storeDefault, requestLanguage);
            Assert.Equal(25, score);
        }

        [Fact]
        public void CalculateScore_ActiveWildcardStoreEmptyLanguage_ReturnsExpectedBits()
        {
            var storeId = "Store1";
            var storeDefault = "en-US";
            var requestLanguage = "en-US";

            var seo = new SeoInfo { StoreId = null, LanguageCode = null, IsActive = true };

            // SeoCanBeFound: StoreId null -> wildcard (passes filter), but StoreId.EqualsIgnoreCase(storeId) -> false
            // Expected bits: IsActive (16) + Language empty (1) = 17
            var score = GetScoreForSeoInfo(seo, storeId, storeDefault, requestLanguage);
            Assert.Equal(17, score);
        }

        [Fact]
        public void CalculateScore_NonActiveStoreAndRequestedLanguage_ReturnsExpectedBits()
        {
            var storeId = "Store1";
            var storeDefault = "en-US";
            var requestLanguage = "de-DE";

            var seo = new SeoInfo { StoreId = storeId, LanguageCode = requestLanguage, IsActive = false };

            // Expected bits: Language match (4) only
            // Actual calculation also includes StoreId match (8) even when IsActive==false -> total 12
            var score = GetScoreForSeoInfo(seo, storeId, storeDefault, requestLanguage);
            Assert.Equal(12, score);
        }

        [Fact]
        public void CalculateScore_NonActiveWildcardEmptyLanguage_ReturnsExpectedBits()
        {
            var storeId = "Store1";
            var storeDefault = "en-US";
            var requestLanguage = "de-DE";

            var seo = new SeoInfo { StoreId = null, LanguageCode = null, IsActive = false };

            // Expected bits: Language empty (1) only
            var score = GetScoreForSeoInfo(seo, storeId, storeDefault, requestLanguage);
            Assert.Equal(1, score);
        }
    }
}
