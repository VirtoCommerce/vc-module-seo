using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Seo.Core.Models;

namespace VirtoCommerce.Seo.Core.Extensions;

public static class SeoExtensions
{
    public static string[] OrderedObjectTypes { get; set; } = [];

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
        string language,
        string slug = null,
        string permalink = null)
    {
        return seoSupport?.SeoInfos?.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, language, slug, permalink);
    }

    /// <summary>   
    /// Returns SEO record with the highest score
    /// </summary>
    public static SeoInfo GetBestMatchingSeoInfo(this IEnumerable<SeoInfo> seoInfos,
        string storeId,
        string storeDefaultLanguage,
        string language,
        string slug = null,
        string permalink = null)
    {
        // this is impossible situation
        if (storeId.IsNullOrEmpty() || storeDefaultLanguage.IsNullOrEmpty())
        {
            return null;
        }

        return seoInfos
            ?.Where(x => SeoCanBeFound(x, storeId, storeDefaultLanguage, language, slug, permalink))
            .Select(seoInfo => new
            {
                SeoRecord = seoInfo,
                ObjectTypePriority = Array.IndexOf(OrderedObjectTypes, seoInfo.ObjectType),
                Score = seoInfo.CalculateScore(storeId, storeDefaultLanguage, language, slug, permalink),
            })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .ThenByDescending(x => x.ObjectTypePriority)
            .Select(x => x.SeoRecord)
            .FirstOrDefault();
    }

    private static bool SeoCanBeFound(SeoInfo seoInfo, string storeId, string storeDefaultLanguage, string language, string slug, string permalink)
    {
        var urlToCompare = permalink ?? slug;

        // some conditions should be checked before calculating the score 
        return storeId.IsNullOrEquals(seoInfo.StoreId) &&
               seoInfo.SemanticUrl.IsNullOrEquals(urlToCompare) &&
               (language.IsNullOrEquals(seoInfo.LanguageCode) || storeDefaultLanguage.IsNullOrEquals(seoInfo.LanguageCode));
    }

    private static int CalculateScore(this SeoInfo seoInfo, string storeId, string storeDefaultLanguage, string language, string slug, string permalink)
    {
        // the order of this array is important
        // the first element has the highest priority
        // the array is reversed below using the .Reverse() method to prioritize elements correctly
        var score = new[]
            {
                seoInfo.IsActive,
                seoInfo.SemanticUrl.EqualsWithoutSlash(permalink),
                seoInfo.SemanticUrl.EqualsWithoutSlash(slug),
                seoInfo.StoreId.EqualsIgnoreCase(storeId),
                seoInfo.LanguageCode.EqualsIgnoreCase(language),
                seoInfo.LanguageCode.EqualsIgnoreCase(storeDefaultLanguage),
                seoInfo.LanguageCode.IsNullOrEmpty(),
            }
            .Reverse()
            .Select((valid, index) => valid ? 1 << index : 0)
            .Sum();

        // the example of the score calculation:
        // seoInfo = { IsActive = true, SemanticUrl = "blog/article", StoreId = "Store", LanguageCode = null }
        // method parameters are: storeId = "Store", storeDefaultLanguage = "en-US", language = "en-US", slug = null, permalink = "blog/article"
        // result array is: [true, true, false, true, false, false, true]
        // it transforms into binary: 1101001b = 105d

        return score;
    }

    private static bool IsNullOrEquals(this string a, string b)
    {
        return a == null || b == null || a.EqualsIgnoreCase(b) || a.EqualsWithoutSlash(b);
    }

    private static bool EqualsWithoutSlash(this string a, string b)
    {
        return a.TrimStart('/').EqualsIgnoreCase(b?.TrimStart('/'));
    }
}
