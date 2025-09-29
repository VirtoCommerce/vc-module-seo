using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Models.SlugInfo;

namespace VirtoCommerce.Seo.Core.Extensions;

public static class SeoExtensions
{
    /// <summary>
    /// Ordered list of object type names used to determine object type priority when comparing SeoInfo entries.
    /// The last element in the list has the highest priority.
    /// This property is mutable via the setter, but the getter returns a read-only snapshot to prevent
    /// accidental runtime mutation which would make the internal priority map stale.
    /// </summary>
    private static string[] _orderedObjectTypes =
    [
        "CatalogProduct",
        "Category",
        "Catalog",
        "Brand",
        "ContentFile",
        "Pages",
    ];

    // Cached concurrent map for object type -> priority index (higher index = higher priority).
    // Rebuilt atomically by assigning a new ConcurrentDictionary instance.
    private static ConcurrentDictionary<string, int> _priorityMap = new(
        _orderedObjectTypes.Select((t, i) => new KeyValuePair<string, int>(t, i)),
        StringComparer.OrdinalIgnoreCase);

    public static IList<string> OrderedObjectTypes
    {
        get
        {
            // Return a read-only wrapper to prevent external mutation of the internal array
            return Array.AsReadOnly(_orderedObjectTypes);
        }
        set
        {
            // Copy input to a new array to avoid external references and publish atomically
            var arr = (value == null) ? [] : value.ToArray();
            _orderedObjectTypes = arr;
            BuildPriorityMap();
        }
    }

    private static void BuildPriorityMap()
    {
        var map = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < _orderedObjectTypes.Length; i++)
        {
            map[_orderedObjectTypes[i]] = i;
        }

        // Publish new map instance atomically
        _priorityMap = map;
    }

    /// <summary>
    /// Creates a fallback <see cref="SeoInfo"/> instance with minimal required fields filled.
    /// Used when no dedicated SeoInfo record exists and a default one must be returned.
    /// </summary>
    /// <param name="id">Semantic URL or identifier to use as fallback.</param>
    /// <param name="name">Name to assign to the fallback SeoInfo.</param>
    /// <param name="cultureName">Language code to assign to the fallback SeoInfo.</param>
    /// <returns>A new <see cref="SeoInfo"/> instance populated with provided values.</returns>
    public static SeoInfo GetFallbackSeoInfo(string id, string name, string cultureName)
    {
        var result = AbstractTypeFactory<SeoInfo>.TryCreateInstance();
        result.SemanticUrl = id;
        result.LanguageCode = cultureName;
        result.Name = name;
        return result;
    }

    /// <summary>
    /// Finds the best matching <see cref="SeoInfo"/> for the given <see cref="ISeoSupport"/> source.
    /// This overload delegates to the enumerable-based implementation using the <see cref="ISeoSupport.SeoInfos"/> collection.
    /// </summary>
    /// <param name="seoSupport">Entity that contains SEO information.</param>
    /// <param name="storeId">Requested store identifier.</param>
    /// <param name="storeDefaultLanguage">Default language of the store.</param>
    /// <param name="language">Requested language (may be null).</param>
    /// <returns>The best matching <see cref="SeoInfo"/>, or <c>null</c> if none matches.</returns>
    public static SeoInfo GetBestMatchingSeoInfo(this ISeoSupport seoSupport,
        string storeId,
        string storeDefaultLanguage,
        string language)
    {
        return seoSupport?.SeoInfos?.GetBestMatchingSeoInfo(storeId, storeDefaultLanguage, language);
    }

    /// <summary>
    /// Evaluates a collection of <see cref="SeoInfo"/> records and returns the one that best matches
    /// the provided store and language parameters according to the scoring and priority rules.
    /// </summary>
    /// <param name="seoInfos">Enumerable of SeoInfo records to evaluate.</param>
    /// <param name="storeId">Requested store identifier.</param>
    /// <param name="storeDefaultLanguage">Default language of the store.</param>
    /// <param name="language">Requested language (may be null).</param>
    /// <returns>The best matching <see cref="SeoInfo"/>, or <c>null</c> if none match.</returns>
    public static SeoInfo GetBestMatchingSeoInfo(this IEnumerable<SeoInfo> seoInfos,
        string storeId,
        string storeDefaultLanguage,
        string language)
    {
        var results = seoInfos.GetSeoInfosResponses(storeId, storeDefaultLanguage, language);

        // Locate final stage and return the first non-null SeoInfo found there (or null)
        var finalStage = results?.FirstOrDefault(x => x.Stage == PipelineStage.Final);
        return finalStage?.SeoInfoResponses?.Where(r => r?.SeoInfo != null).Select(r => r.SeoInfo).FirstOrDefault();
    }

    /// <summary>
    /// Runs the multi-stage evaluation pipeline over the provided SeoInfo collection and returns
    /// a list of stage results. Each stage entry contains a snapshot of the evaluated responses
    /// so the caller can inspect filtering, scoring and ordering steps for diagnostics or testing.
    /// </summary>
    public static IList<SeoInfosResponse> GetSeoInfosResponses(this IEnumerable<SeoInfo> enumerable,
        string storeId,
        string storeDefaultLanguage,
        string language)
    {
        if (storeId.IsNullOrEmpty() || storeDefaultLanguage.IsNullOrEmpty() || enumerable == null)
        {
            return null;
        }

        var seoInfos = enumerable.ToList();
        if (seoInfos.Count == 0)
        {
            return null;
        }

        var results = new List<SeoInfosResponse>();

        // Create initial responses
        IList<SeoInfoResponse> current = seoInfos.Select(seoInfo => seoInfo.ToSeoInfoResponse()).ToList();
        // store immutable snapshots for each stage
        results.Add(new SeoInfosResponse(PipelineStage.Original, "Stage 1: Original found by SeoInfo.", current.ToList().AsReadOnly()));

        current = current.FilterCanBeFound(storeId, storeDefaultLanguage, language);
        results.Add(new SeoInfosResponse(PipelineStage.Filtered, "Stage 2: Filtering is there seo.", current.ToList().AsReadOnly()));

        current = current.CalculateScores(storeId, storeDefaultLanguage, language);
        results.Add(new SeoInfosResponse(PipelineStage.Scored, "Stage 3: Calculate scores.", current.ToList().AsReadOnly()));

        current = current.FilterScoresGreaterThanZero();
        results.Add(new SeoInfosResponse(PipelineStage.FilteredScore, "Stage 4: Filter score greater than 0.", current.ToList().AsReadOnly()));

        current = current.OrderScoresAndPriority();
        results.Add(new SeoInfosResponse(PipelineStage.Ordered, "Stage 5: Order by score, then order by desc objectTypePriority.", current.ToList().AsReadOnly()));

        // Only include non-null final candidate(s)
        var finalList = current.Where(x => x != null).Take(1).ToList();
        results.Add(new SeoInfosResponse(PipelineStage.Final, "Stage 6: Select first or default SeoInfo.", finalList.AsReadOnly()));

        return results;
    }

    /// <summary>
    /// Converts <see cref="SeoInfo"/> to an evaluation response model.
    /// </summary>
    /// <param name="seoInfo">Source SeoInfo.</param>
    /// <param name="objectTypePriority">Optional object type priority to set on the response.</param>
    /// <param name="score">Optional score to set on the response.</param>
    /// <returns>New <see cref="SeoInfoResponse"/> instance representing the SeoInfo in the pipeline.</returns>
    private static SeoInfoResponse ToSeoInfoResponse(this SeoInfo seoInfo, int objectTypePriority = 0, int score = 0)
    {
        return new SeoInfoResponse(seoInfo, objectTypePriority, score);
    }

    /// <summary>
    /// Filters out SeoInfoResponse entries that do not match the provided store and language criteria.
    /// </summary>
    /// <param name="seoInfoResponses">Collection of responses to filter.</param>
    /// <param name="storeId">Requested store identifier.</param>
    /// <param name="storeDefaultLanguage">Default language of the store.</param>
    /// <param name="language">Requested language (may be null).</param>
    /// <returns>Filtered list of responses that can be used for scoring.</returns>
    private static IList<SeoInfoResponse> FilterCanBeFound(this IList<SeoInfoResponse> seoInfoResponses, string storeId, string storeDefaultLanguage, string language)
    {
        return seoInfoResponses.Where(seoInfoResponse => SeoCanBeFound(seoInfoResponse.SeoInfo, storeId, storeDefaultLanguage, language)).ToList();
    }

    /// <summary>
    /// Calculates object type priority and score for each response in the collection.
    /// Object type priority is resolved from <see cref="OrderedObjectTypes"/> and score is computed
    /// using <see cref="CalculateScore(SeoInfo,string,string,string)"/>.
    /// </summary>
    /// <param name="seoInfoResponses">Collection of responses to enrich with scores and priorities.</param>
    /// <param name="storeId">Requested store identifier.</param>
    /// <param name="storeDefaultLanguage">Default language of the store.</param>
    /// <param name="language">Requested language (may be null).</param>
    /// <returns>New list of responses populated with priority and score values.</returns>
    private static IList<SeoInfoResponse> CalculateScores(this IList<SeoInfoResponse> seoInfoResponses, string storeId, string storeDefaultLanguage, string language)
    {
        var map = _priorityMap; // snapshot to avoid races if OrderedObjectTypes changes concurrently

        return seoInfoResponses.Select(seoInfoResponse =>
        {
            // handle null entries gracefully
            var info = seoInfoResponse?.SeoInfo;
            if (info == null)
            {
                return new SeoInfoResponse(null, -1, 0);
            }

            var objectTypeKey = info.ObjectType ?? string.Empty;
            var priority = map != null && map.TryGetValue(objectTypeKey, out var idx) ? idx : -1;
            var score = info.CalculateScore(storeId, storeDefaultLanguage, language);

            return new SeoInfoResponse(info, priority, score);
        }).ToList();
    }

    /// <summary>
    /// Filters out responses with non-positive score.
    /// </summary>
    /// <param name="seoInfoResponses">Collection of responses to filter.</param>
    /// <returns>Responses that have a score greater than zero.</returns>
    private static IList<SeoInfoResponse> FilterScoresGreaterThanZero(this IList<SeoInfoResponse> seoInfoResponses)
    {
        return seoInfoResponses.Where(x => x.Score > 0).ToList();
    }

    /// <summary>
    /// Orders responses first by score (descending) and then by object type priority (descending).
    /// </summary>
    /// <param name="seoInfoResponses">Collection of responses to order.</param>
    /// <returns>Ordered list of responses.</returns>
    private static IList<SeoInfoResponse> OrderScoresAndPriority(this IList<SeoInfoResponse> seoInfoResponses)
    {
        return seoInfoResponses
            .OrderByDescending(x => x.Score)
            .ThenByDescending(x => x.ObjectTypePriority)
            .ToList();
    }

    /// <summary>
    /// Checks whether a given SeoInfo can be considered for matching according to store and language criteria.
    /// Note: empty or null store/language values are treated as wildcard matches.
    /// </summary>
    /// <param name="seoInfo">SeoInfo to check.</param>
    /// <param name="storeId">Requested store identifier.</param>
    /// <param name="storeDefaultLanguage">Default language of the store.</param>
    /// <param name="language">Requested language (may be null).</param>
    /// <returns><c>true</c> if SeoInfo matches criteria; otherwise <c>false</c>.</returns>
    private static bool SeoCanBeFound(SeoInfo seoInfo, string storeId, string storeDefaultLanguage, string language)
    {
        return seoInfo.StoreId.Matches(storeId) &&
               seoInfo.LanguageCode.MatchesAny(storeDefaultLanguage, language);
    }

    // Score bits as named flags for readability
    [Flags]
    private enum ScoreFactor
    {
        LanguageEmpty = 1 << 0,         // 1
        LanguageStoreDefault = 1 << 1,  // 2
        LanguageExact = 1 << 2,         // 4
        StoreMatch = 1 << 3,            // 8
        IsActive = 1 << 4,              // 16
    }

    /// <summary>
    /// Calculates integer score for a SeoInfo based on several boolean factors (active, store match, language match etc.).
    /// The score is computed as a bit-encoded value where each condition contributes a named flag.
    /// </summary>
    /// <param name="seoInfo">SeoInfo to evaluate.</param>
    /// <param name="storeId">Requested store identifier.</param>
    /// <param name="storeDefaultLanguage">Default language of the store.</param>
    /// <param name="language">Requested language (may be null).</param>
    /// <returns>Integer score representing how well SeoInfo matches the requested parameters.</returns>
    private static int CalculateScore(this SeoInfo seoInfo, string storeId, string storeDefaultLanguage, string language)
    {
        var score = 0;

        if (seoInfo.IsActive)
        {
            score |= (int)ScoreFactor.IsActive;
        }

        if (seoInfo.StoreId.EqualsIgnoreCase(storeId))
        {
            score |= (int)ScoreFactor.StoreMatch;
        }

        if (seoInfo.LanguageCode.EqualsIgnoreCase(language))
        {
            score |= (int)ScoreFactor.LanguageExact;
        }

        if (seoInfo.LanguageCode.EqualsIgnoreCase(storeDefaultLanguage))
        {
            score |= (int)ScoreFactor.LanguageStoreDefault;
        }

        if (seoInfo.LanguageCode.IsNullOrEmpty())
        {
            score |= (int)ScoreFactor.LanguageEmpty;
        }

        return score;
    }

    /// <summary>
    /// Returns <c>true</c> if string <paramref name="a"/> matches either <paramref name="b"/> or <paramref name="c"/>.
    /// The comparison uses <see cref="Matches(string,string)"/>, which treats null/empty as wildcard matches.
    /// </summary>
    private static bool MatchesAny(this string a, string b, string c)
    {
        return a.Matches(b) || a.Matches(c);
    }

    /// <summary>
    /// Compares two strings using case-insensitive comparison, treating null or empty values as wildcards
    /// (i.e. if either string is null/empty the method returns true).
    /// </summary>
    private static bool Matches(this string a, string b)
    {
        return a.IsNullOrEmpty() || b.IsNullOrEmpty() || a.EqualsIgnoreCase(b);
    }
}
