using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Models.Explain;

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
    /// Evaluates a collection of <see cref="SeoInfo"/> records and selects the best match according to the
    /// configured scoring rules and object-type priorities.
    /// The selection  typically performs: filtering by store/language → scoring → filtering by positive score → ordering → selecting first.
    /// </summary>
    public static SeoInfo GetBestMatchingSeoInfo(this IEnumerable<SeoInfo> seoInfos,
        string storeId,
        string storeDefaultLanguage,
        string language)
    {
        var (seoInfo, _) = seoInfos.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, language, explain: false);

        return seoInfo;
    }

    /// <summary>
    /// Executes the explainable evaluation  over the provided <see cref="SeoInfo"/> collection.
    /// Returns a tuple where <c>explainResults</c> is a list of explain results (snapshots of each stage) and
    /// <c>SeoInfo</c> is the selected best matching SeoInfo (can be null).
    /// This method is intended to make the selection process introspectable for diagnostics and tests.
    /// </summary>
    public static (SeoInfo, IList<SeoExplainResult>) GetBestMatchingSeoInfo(this IEnumerable<SeoInfo> seoInfos,
        string storeId,
        string storeDefaultLanguage,
        string language,
        bool explain)
    {
        if (storeId.IsNullOrEmpty() || storeDefaultLanguage.IsNullOrEmpty() || seoInfos == null)
        {
            return (null, explain ? [] : null);
        }

        var seoInfoList = seoInfos as IList<SeoInfo> ?? seoInfos.ToList();
        if (seoInfoList.Count == 0)
        {
            return (null, explain ? [] : null);
        }

        List<SeoExplainResult> explainResults = null;

        // Stage 1: Original - snapshot of found SeoInfo records (no scores or priorities yet)
        var stageOriginal = seoInfoList
            .Select(seoInfo => new SeoExplainItem(seoInfo));
        stageOriginal = AddExplain(SeoExplainStage.Original, stageOriginal);

        // Stage 2: Filtered - keep only entries that match store and language criteria
        var stageFiltered = stageOriginal
            .Where(candidate => SeoCanBeFound(candidate.SeoInfo, storeId, storeDefaultLanguage, language));
        stageFiltered = AddExplain(SeoExplainStage.Filtered, stageFiltered);

        // Stage 3: Scored - compute object type priority and numeric score for each candidate
        var stageScored = stageFiltered
            .CalculatePriorityAndScores(storeId, storeDefaultLanguage, language, explain);
        stageScored = AddExplain(SeoExplainStage.Scored, stageScored);

        // Stage 4: FilteredScore - remove entries with non-positive score
        var stageFilteredScore = stageScored
            .Where(candidate => candidate.Score > 0);
        stageFilteredScore = AddExplain(SeoExplainStage.FilteredScore, stageFilteredScore);

        // Stage 5: Ordered - order by score (desc) then by object type priority (desc)
        var stageOrdered = stageFilteredScore
            .OrderByDescending(candidate => candidate.Score)
            .ThenByDescending(candidate => candidate.ObjectTypePriority)
            .AsEnumerable();
        stageOrdered = AddExplain(SeoExplainStage.Ordered, stageOrdered);

        // Stage 6: Final - take first candidate (if any)
        var stageFinal = stageOrdered
            .Where(candidate => candidate.SeoInfo != null)
            .Take(1);
        stageFinal = AddExplain(SeoExplainStage.Final, stageFinal);

        var bestMatchingSeoInfo = stageFinal.FirstOrDefault()?.SeoInfo;

        return (bestMatchingSeoInfo, explainResults);

        IEnumerable<SeoExplainItem> AddExplain(SeoExplainStage stage, IEnumerable<SeoExplainItem> items)
        {
            // To reduce memory usage, avoid allocating and populating the explainResults list when an explanation is not requested.
            if (!explain)
            {
                return items;
            }

            explainResults ??= [];

            var list = items.ToList();
            explainResults.Add(new SeoExplainResult(stage, list));
            return list;
        }
    }

    /// <summary>
    /// For each input item calculates object type priority (index in <see cref="OrderedObjectTypes"/>) and score.
    /// If a <see cref="SeoExplainItem"/>'s SeoInfo is null it is preserved with a priority of -1 and score 0 to keep snapshots stable.
    /// </summary>
    private static IEnumerable<SeoExplainItem> CalculatePriorityAndScores(this IEnumerable<SeoExplainItem> seoExplainItems,
        string storeId,
        string storeDefaultLanguage,
        string language,
        bool explain)
    {
        foreach (var item in seoExplainItems)
        {
            var mutableItem = new SeoExplainItem(item.SeoInfo);

            if (mutableItem.SeoInfo != null)
            {
                // Resolve object type priority using configured OrderedObjectTypes. If type is not found, priority is -1.
                mutableItem.ObjectTypePriority = Array.IndexOf(OrderedObjectTypes, mutableItem.SeoInfo.ObjectType);
                mutableItem.Score = mutableItem.SeoInfo.CalculateScore(storeId, storeDefaultLanguage, language);
            }

            yield return mutableItem;
        }
    }

    /// <summary>
    /// Determines whether the provided SeoInfo matches the store and language filtering rules.
    /// Treats null or empty values as wildcards (matches everything).
    /// </summary>
    private static bool SeoCanBeFound(
        SeoInfo seoInfo,
        string storeId,
        string storeDefaultLanguage,
        string language)
    {
        if (seoInfo == null)
        {
            return false;
        }

        return seoInfo.StoreId.Matches(storeId) &&
               seoInfo.LanguageCode.MatchesAny(storeDefaultLanguage, language);
    }

    private static int CalculateScore(this SeoInfo seoInfo,
        string storeId,
        string storeDefaultLanguage,
        string language)
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
