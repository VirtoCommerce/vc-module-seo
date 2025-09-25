using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Seo.Core.Models;

namespace VirtoCommerce.Seo.Core.Extensions;

public static class SeoExtensions
{
    // the last element in the array has the highest priority
    public static string[] OrderedObjectTypes { get; set; } =
    [
        "CatalogProduct",
        "Category",
        "Catalog",
        "Brand",
        "ContentFile",
        "Pages",
    ];

    public static SeoInfo GetFallbackSeoInfo(string id, string name, string cultureName)
    {
        var result = AbstractTypeFactory<SeoInfo>.TryCreateInstance();
        result.SemanticUrl = id;
        result.LanguageCode = cultureName;
        result.Name = name;
        return result;
    }

    /// <summary>
    /// Returns SEO record with the highest score
    /// </summary>
    public static SeoInfo GetBestMatchingSeoInfo(this ISeoSupport seoSupport,
        string storeId,
        string storeDefaultLanguage,
        string language)
    {
        return seoSupport?.SeoInfos?.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, language);
    }

    /// <summary>
    /// Returns SEO record with the highest score
    /// </summary>
    public static SeoInfo GetBestMatchingSeoInfo(this IEnumerable<SeoInfo> seoInfos,
        string storeId,
        string storeDefaultLanguage,
        string language)
    {
        // this is impossible situation
        if (storeId.IsNullOrEmpty() || storeDefaultLanguage.IsNullOrEmpty())
        {
            return null; // ToDo: Unexpected behavior
        }

        return seoInfos
            .FilterSeoInfoCanBeFound(storeId, storeDefaultLanguage, language)
            .SelectSeoInfoScores(storeId, storeDefaultLanguage, language)
            .FilterSeoInfoScoresGreaterThenZero()
            .OrderSeoInfoScores()
            .SelectSeoInfos()
            .FirstOrDefault();
    }

    public static IEnumerable<SeoInfo> FilterSeoInfoCanBeFound(this IEnumerable<SeoInfo> seoInfos, string storeId, string storeDefaultLanguage, string language) =>
        seoInfos?.Where(x => SeoCanBeFound(x, storeId, storeDefaultLanguage, language));

    public static IEnumerable<SeoInfoScored> SelectSeoInfoScores(this IEnumerable<SeoInfo> seoInfos, string storeId, string storeDefaultLanguage, string language)
    {
        return seoInfos.Select(seoInfo => new SeoInfoScored
        {
            SeoRecord = seoInfo,
            ObjectTypePriority = Array.IndexOf(OrderedObjectTypes, seoInfo.ObjectType),
            Score = seoInfo.CalculateScore(storeId, storeDefaultLanguage, language),
        });
    }

    public static IEnumerable<SeoInfoScored> FilterSeoInfoScoresGreaterThenZero(this IEnumerable<SeoInfoScored> seoInfoScores) =>
        seoInfoScores.Where(x => x.Score > 0);

    public static IEnumerable<SeoInfoScored> OrderSeoInfoScores(this IEnumerable<SeoInfoScored> seoInfoScores) =>
        seoInfoScores
            .OrderByDescending(x => x.Score)
            .ThenByDescending(x => x.ObjectTypePriority);

    public static IEnumerable<SeoInfo> SelectSeoInfos(this IEnumerable<SeoInfoScored> seoInfoScores) =>
        seoInfoScores.Select(x => x.SeoRecord);

    private static bool SeoCanBeFound(SeoInfo seoInfo, string storeId, string storeDefaultLanguage, string language)
    {
        // some conditions should be checked before calculating the score
        return seoInfo.StoreId.Matches(storeId) &&
               seoInfo.LanguageCode.MatchesAny(storeDefaultLanguage, language);
    }

    private static int CalculateScore(this SeoInfo seoInfo, string storeId, string storeDefaultLanguage, string language)
    {
        // the order of this array is important
        // the first element has the highest priority
        // the array is reversed below using the .Reverse() method to prioritize elements correctly
        var score = new[]
            {
                seoInfo.IsActive,
                seoInfo.StoreId.EqualsIgnoreCase(storeId),
                seoInfo.LanguageCode.EqualsIgnoreCase(language),
                seoInfo.LanguageCode.EqualsIgnoreCase(storeDefaultLanguage),
                seoInfo.LanguageCode.IsNullOrEmpty(),
            }
            .Reverse()
            .Select((valid, index) => valid ? 1 << index : 0)
            .Sum();

        // the example of the score calculation:
        // seoInfo = { IsActive = true, StoreId = "Store", LanguageCode = null }
        // method parameters are: storeId = "Store", storeDefaultLanguage = "en-US", language = "en-US"
        // result array is: [IsActive:true, StoreId:true, language:false, storeLanguage:false, seoLanguage:true]
        // it transforms into binary: 10011b = 19d

        return score;
    }

    private static bool MatchesAny(this string a, string b, string c)
    {
        return a.Matches(b) || a.Matches(c);
    }

    private static bool Matches(this string a, string b)
    {
        return a.IsNullOrEmpty() || b.IsNullOrEmpty() || a.EqualsIgnoreCase(b);
    }
}
