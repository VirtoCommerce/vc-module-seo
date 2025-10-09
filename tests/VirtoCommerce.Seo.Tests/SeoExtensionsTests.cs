using System.Collections.Generic;
using VirtoCommerce.Seo.Core.Extensions;
using VirtoCommerce.Seo.Core.Models;
using Xunit;

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

            var defaultTypes = SeoExtensions.OrderedObjectTypes;

            SeoExtensions.OrderedObjectTypes = ["Categories", "Pages"];

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

            SeoExtensions.OrderedObjectTypes = defaultTypes;
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
        public void GetBestMatchingInfo_Brands_WithStore()
        {
            // Arrange
            var storeId = "Store1";
            var storeDefaultLanguage = "en-US";

            var seoInfos = new List<SeoInfo>
            {
                new() { ObjectType = "Catalog", StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "brands"},
                new() { ObjectType = "Brands", StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "brands"},
            };

            // Act
            var result = seoInfos.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, language: null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Brands", result.ObjectType);
        }

        [Fact]
        public void GetBestMatchingInfo_Brands_WithoutStore()
        {
            // Arrange
            var storeId = "Store1";
            var storeDefaultLanguage = "en-US";

            var seoInfos = new List<SeoInfo>
            {
                new() { ObjectType = "Catalog", StoreId = null, LanguageCode = "en-US", SemanticUrl = "brands"},
                new() { ObjectType = "Brands", StoreId = "Store1", LanguageCode = "en-US", SemanticUrl = "brands"},
            };

            // Act
            var result = seoInfos.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, language: null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Brands", result.ObjectType);
        }
    }
}
