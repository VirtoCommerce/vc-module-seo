using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Models.Explain;
using VirtoCommerce.Seo.Core.Models.Explain.Enums;

namespace VirtoCommerce.Seo.Core.Extensions;

public static class SeoExtensions
{
    /// <summary>
    /// Ordered list of object type names used to determine object type priority when comparing <see cref="SeoInfo"/> entries.
    /// The last element in the array has the highest priority. Applications can override this ordering to change which
    /// object types are preferred when multiple SeoInfo records have identical scores. This property represents the
    /// configured ordering and should be treated as configuration data rather than runtime state.
    /// </summary>
    public static string[] OrderedObjectTypes =
    [
        "CatalogProduct",
        "Category",
        "Catalog",
        "Brand",
        "ContentFile",
        "Pages",
    ];

    /// <summary>
    /// Creates a minimal fallback <see cref="SeoInfo"/> instance populated with the provided values.
    /// Use this when no dedicated SeoInfo record exists and a default, synthetic SeoInfo must be returned to callers.
    /// The returned instance contains only the fields necessary to identify a fallback result (SemanticUrl, Name, LanguageCode).
    /// </summary>
    /// <param name="id">The semantic URL or identifier to use as the fallback value.</param>
    /// <param name="name">The display name for the fallback SeoInfo.</param>
    /// <param name="cultureName">Language code to assign to the fallback SeoInfo.</param>
    /// <returns>A new instance of <see cref="SeoInfo"/> initialized with the supplied values.</returns>
    public static SeoInfo GetFallbackSeoInfo(string id, string name, string cultureName)
    {
        var result = AbstractTypeFactory<SeoInfo>.TryCreateInstance();
        result.SemanticUrl = id;
        result.LanguageCode = cultureName;
        result.Name = name;
        return result;
    }

    /// <summary>
    /// Convenience overload that finds the best matching <see cref="SeoInfo"/> for an <see cref="ISeoSupport"/> entity.
    /// Delegates to the enumerable-based resolver using the entity's <see cref="ISeoSupport.SeoInfos"/> collection.
    /// </summary>
    /// <param name="seoSupport">Entity that contains SEO information (may be null).</param>
    /// <param name="storeId">Requested store identifier (used for scoping).</param>
    /// <param name="storeDefaultLanguage">Default language of the store (used as a language fallback).</param>
    /// <param name="language">Requested language code (may be null).</param>
    /// <returns>The best matching <see cref="SeoInfo"/>, or <c>null</c> if no candidate matches.</returns>
    public static SeoInfo GetBestMatchingSeoInfo(this ISeoSupport seoSupport,
        string storeId,
        string storeDefaultLanguage,
        string language)
    {
        return seoSupport?.SeoInfos?.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, language);
    }

    /// <summary>
    /// Evaluate a collection of <see cref="SeoInfo"/> records and select the best match according to the
    /// configured scoring rules and object-type priorities.
    /// The selection pipeline typically performs: filtering by store/language → scoring → filtering by positive score → ordering → selecting first.
    /// </summary>
    /// <param name="seoInfos">Candidates to evaluate.</param>
    /// <param name="storeId">Requested store identifier.</param>
    /// <param name="storeDefaultLanguage">Default language of the store - used when requested language is not available.</param>
    /// <param name="language">Requested language code (may be null).</param>
    /// <returns>The best matching <see cref="SeoInfo"/>, or <c>null</c> when none matched.</returns>
    public static SeoInfo GetBestMatchingSeoInfo(this IEnumerable<SeoInfo> seoInfos,
        string storeId,
        string storeDefaultLanguage,
        string language)
    {
        var explainResult = seoInfos.GetSeoInfoExplain(storeId, storeDefaultLanguage, language);

        return explainResult.SeoInfo;
    }

    /// <summary>
    /// Executes the explainable evaluation pipeline over the provided <see cref="SeoInfo"/> collection.
    /// Returns a tuple where <c>Results</c> is a list of explain results (snapshots of each stage) and
    /// <c>SeoInfo</c> is the selected best matching SeoInfo (may be null).
    /// This method is intended to make the selection process introspectable for diagnostics and tests.
    /// </summary>
    /// <param name="enumerable">Input collection of SeoInfo records to process.</param>
    /// <param name="storeId">Requested store identifier.</param>
    /// <param name="storeDefaultLanguage">Store default language used for fallback.</param>
    /// <param name="language">Requested language code (may be null).</param>
    /// <param name="withExplain">When true, the returned Results list contains full snapshots for each pipeline stage.</param>
    /// <returns>Tuple containing a list of <see cref="SeoExplainResult"/> and the chosen <see cref="SeoInfo"/>.</returns>
    public static (IList<SeoExplainResult> Results, SeoInfo SeoInfo) GetSeoInfoExplain(this IEnumerable<SeoInfo> enumerable,
          string storeId,
        string storeDefaultLanguage,
        string language,
        bool withExplain = false)
    {
        if (storeId.IsNullOrEmpty() || storeDefaultLanguage.IsNullOrEmpty() || enumerable == null)
        {
            return (null, null);
        }

        var seoInfos = enumerable.ToList();
        if (seoInfos.Count == 0)
        {
            return (null, null);
        }

        // Prepare explain results container (filled only when withExplain==true)
        var explainResults = new List<SeoExplainResult>();

        // Stage 1: Original - snapshot of found SeoInfo records (no scores or priorities yet)
        var stageOriginal = seoInfos
            .Select(seoInfo => (SeoInfo: seoInfo, ObjectTypePriority: 0, Score: 0))
            .ToList();
        if (withExplain)
        {
            explainResults.Add(new SeoExplainResult(SeoExplainPipelineStage.Original, stageOriginal));
        }

        // Stage 2: Filtered - keep only entries that match store and language criteria
        var stageFiltered = stageOriginal
            .Where(candidate => SeoCanBeFound(candidate.SeoInfo, storeId, storeDefaultLanguage, language))
            .ToList();
        if (withExplain)
        {
            explainResults.Add(new SeoExplainResult(SeoExplainPipelineStage.Filtered, stageFiltered));
        }

        // Stage 3: Scored - compute object type priority and numeric score for each candidate
        var stageScored = stageFiltered.CalculatePriorityAndScores(storeId, storeDefaultLanguage, language).ToList();
        if (withExplain)
        {
            explainResults.Add(new SeoExplainResult(SeoExplainPipelineStage.Scored, stageScored));
        }

        // Stage 4: FilteredScore - remove entries with non-positive score
        var stageFilteredScore = stageScored.Where(candidate => candidate.Score > 0).ToList();
        if (withExplain)
        {
            explainResults.Add(new SeoExplainResult(SeoExplainPipelineStage.FilteredScore, stageFilteredScore));
        }

        // Stage 5: Ordered - order by score (desc) then by object type priority (desc)
        var stageOrdered = stageFilteredScore
            .OrderByDescending(candidate => candidate.Score)
            .ThenByDescending(candidate => candidate.ObjectTypePriority)
            .ToList();
        if (withExplain)
        {
            explainResults.Add(new SeoExplainResult(SeoExplainPipelineStage.Ordered, stageOrdered));
        }

        // Stage 6: Final - take first candidate (if any)
        var stageFinal = stageOrdered.Where(candidate => candidate.SeoInfo != null).Take(1).ToList();
        if (withExplain)
        {
            explainResults.Add(new SeoExplainResult(SeoExplainPipelineStage.Final, stageFinal));
        }

        var selectedSeoInfo = stageFinal.FirstOrDefault().SeoInfo; // safe: FirstOrDefault returns default tuple when empty

        // When explain is not requested, avoid allocating and returning the explainResults list to reduce memory usage.
        return (withExplain ? explainResults : null, selectedSeoInfo);
    }

    /// <summary>
    /// For each input tuple calculates object type priority (index in <see cref="OrderedObjectTypes"/>) and score.
    /// If a tuple's SeoInfo is null it is preserved with a priority of -1 and score 0 to keep pipeline snapshots stable.
    /// </summary>
    /// <param name="tuples">Input list of tuples where each tuple contains a SeoInfo to evaluate.</param>
    /// <param name="storeId">Requested store identifier.</param>
    /// <param name="storeDefaultLanguage">Default language of the store.</param>
    /// <param name="language">Requested language (may be null).</param>
    /// <returns>List of tuples with calculated ObjectTypePriority and Score values populated.</returns>
    private static IList<(SeoInfo SeoInfo, int ObjectTypePriority, int Score)> CalculatePriorityAndScores(this IList<(SeoInfo SeoInfo, int ObjectTypePriority, int Score)> tuples,
        string storeId,
        string storeDefaultLanguage,
        string language)
    {
        var results = new List<(SeoInfo SeoInfo, int ObjectTypePriority, int Score)>();

        foreach (var item in tuples)
        {
            var seoInfo = item.SeoInfo;

            if (seoInfo == null)
            {
                // Preserve null candidates with explicit priority -1 and score 0
                results.Add((null, -1, 0));
                continue;
            }

            // Resolve object type priority using configured OrderedObjectTypes. If type not found, priority is -1.
            var priority = Array.IndexOf(OrderedObjectTypes, seoInfo.ObjectType);
            var score = seoInfo.CalculateScore(storeId, storeDefaultLanguage, language);

            results.Add((seoInfo, priority, score));
        }

        return results;
    }

    /// <summary>
    /// Determines whether the provided SeoInfo matches the store and language filtering rules.
    /// Treats null or empty values as wildcards (matches everything).
    /// </summary>
    /// <param name="seoInfo">Candidate SeoInfo to test.</param>
    /// <param name="storeId">Requested store identifier.</param>
    /// <param name="storeDefaultLanguage">Default language of the store (for fallback matching).</param>
    /// <param name="language">Requested language (may be null).</param>
    /// <returns><c>true</c> when the SeoInfo should be included for scoring; otherwise <c>false</c>.</returns>
    private static bool SeoCanBeFound(
        SeoInfo seoInfo,
        string storeId,
        string storeDefaultLanguage,
        string language)
    {
        return seoInfo.StoreId.Matches(storeId) &&
               seoInfo.LanguageCode.MatchesAny(storeDefaultLanguage, language);
    }

    private static int CalculateScore(this SeoInfo seoInfo,
        string storeId,
        string storeDefaultLanguage,
        string language)
    {
        // Score is built from a set of boolean attributes where each 'true' contributes a bit.
        // The array order matters as it is reversed to ensure the first element has the highest binary weight.
        var score = new[]
            {
                seoInfo.IsActive,
                seoInfo.StoreId.EqualsIgnoreCase(storeId),
                seoInfo.LanguageCode.EqualsIgnoreCase(language),
                seoInfo.LanguageCode.EqualsIgnoreCase(storeDefaultLanguage),
                seoInfo.LanguageCode.IsNullOrEmpty(),
            }
            .Reverse()
            .Select((isValid, index) => isValid ? 1 << index : 0)
            .Sum();

        // Example: IsActive=true, StoreId match=true, Language not match, Store default not match, Language empty=true -> binary 10011 = 19
        return score;
    }

    /// <summary>
    /// Returns true if the left-hand string equals either of the two provided strings.
    /// Uses <see cref="Matches(string,string)"/> semantics where null/empty act as wildcards.
    /// </summary>
    private static bool MatchesAny(this string a, string b, string c)
    {
        return a.Matches(b) || a.Matches(c);
    }

    /// <summary>
    /// Case-insensitive comparison that treats null or empty values as wildcards (matches anything).
    /// Returns true when either operand is null or empty or when the strings are equal ignoring case.
    /// </summary>
    private static bool Matches(this string a, string b)
    {
        return a.IsNullOrEmpty() || b.IsNullOrEmpty() || a.EqualsIgnoreCase(b);
    }
}
