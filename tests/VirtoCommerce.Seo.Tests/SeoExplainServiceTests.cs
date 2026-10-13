using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Models.Explain;
using VirtoCommerce.Seo.Core.Services;
using VirtoCommerce.Seo.Data.Services;
using Xunit;

namespace VirtoCommerce.Seo.Tests;

public class SeoExplainServiceTests
{
    private const string StoreId = "store-1";
    private const string Language = "en-US";
    private const string Permalink = "v-dresses";

    [Fact]
    public async Task ExplainAsync_RecordRejectedByResolver_ReturnsCandidateWithReasonAndEmptyStages()
    {
        // VCST-5989: the record exists, but the resolver rejects it for this store.
        // The Candidates stage shows it with the reason; the six pipeline stages are still returned, all empty,
        // so the response differs from the "slug exists nowhere" case below.
        var resolver = new ExplainingResolver(resolved: [], candidates: [CreateCandidate("seo-1", "NotInStoreCatalog")]);
        var service = CreateService(resolver);

        var result = await service.ExplainAsync(StoreId, null, Language, Language, Permalink);

        Assert.Equal(7, result.Count);
        Assert.Equal(SeoExplainStage.Candidates, result[0].Stage);
        var candidate = Assert.Single(result[0].Items);
        Assert.False(candidate.IsResolved);
        Assert.Equal("NotInStoreCatalog", Assert.Single(candidate.Reasons).Code);
        Assert.Equal(SeoExplainStage.Original, result[1].Stage);
        Assert.All(result.Skip(1), x => Assert.Empty(x.Items));
    }

    [Fact]
    public async Task ExplainAsync_SlugExistsNowhere_ReturnsNoStages()
    {
        // No resolver knows the slug at all: nothing to explain, the UI shows "Nothing found".
        var resolver = new ExplainingResolver(resolved: [], candidates: []);
        var service = CreateService(resolver);

        var result = await service.ExplainAsync(StoreId, null, Language, Language, "no-such-slug-at-all");

        Assert.Empty(result);
    }

    [Fact]
    public async Task ExplainAsync_ResolvedRecord_ReturnsCandidatesAndFullPipeline()
    {
        // Control case: a resolved record is a resolved candidate and passes every pipeline stage.
        var resolver = new ExplainingResolver(resolved: [CreateSeoInfo("seo-1")], candidates: [CreateCandidate("seo-1")]);
        var service = CreateService(resolver);

        var result = await service.ExplainAsync(StoreId, null, Language, Language, Permalink);

        Assert.Equal(7, result.Count);
        var candidate = Assert.Single(result[0].Items);
        Assert.True(candidate.IsResolved);
        Assert.All(result.Skip(1), x => Assert.Equal("seo-1", Assert.Single(x.Items).SeoInfo.Id));
    }

    [Fact]
    public async Task ExplainAsync_ResolverWithDefaultGetCandidatesAsync_ListsItsResolvedRecords()
    {
        // A module that doesn't override ISeoResolver.GetCandidatesAsync (Store, News, a partner module compiled
        // before it existed) still shows what it resolved, through the interface default.
        var resolver = new Resolver(resolved: [CreateSeoInfo("seo-1")]);
        var service = CreateService(resolver);

        var result = await service.ExplainAsync(StoreId, null, Language, Language, Permalink);

        var candidate = Assert.Single(result[0].Items);
        Assert.True(candidate.IsResolved);
        Assert.Equal("seo-1", Assert.Single(result[1].Items).SeoInfo.Id);
    }

    [Fact]
    public async Task ExplainAsync_CustomCompositeResolver_ExplainsItsResult()
    {
        // A project may replace ICompositeSeoResolver with its own class that has only FindSeoAsync.
        // Explain must keep working for it, as it did before GetCandidatesAsync existed.
        var service = new SeoExplainService(new CustomCompositeResolver(resolved: [CreateSeoInfo("seo-1")]));

        var result = await service.ExplainAsync(StoreId, null, Language, Language, Permalink);

        Assert.True(Assert.Single(result[0].Items).IsResolved);
        Assert.Equal("seo-1", Assert.Single(result.Last().Items).SeoInfo.Id);
    }

    [Fact]
    public async Task ExplainAsync_RecordFromTwoResolvers_IsListedOnceAsResolved()
    {
        // Two resolvers may return the same record. CompositeSeoResolver.FindSeoAsync lists it once, so the Candidates
        // stage does too, and it stays resolved even though the other resolver rejected it.
        var rejecting = new ExplainingResolver(resolved: [], candidates: [CreateCandidate("seo-1", "NotInStoreCatalog")]);
        var resolving = new Resolver(resolved: [CreateSeoInfo("seo-1")]);
        var service = new SeoExplainService(new CompositeSeoResolver([rejecting, resolving], new EventPublisher()));

        var result = await service.ExplainAsync(StoreId, null, Language, Language, Permalink);

        Assert.True(Assert.Single(result[0].Items).IsResolved);
        Assert.Equal("seo-1", Assert.Single(result[1].Items).SeoInfo.Id);
    }

    [Fact]
    public async Task ExplainAsync_AsksResolversForUpToHundredCandidates()
    {
        // Resolvers cap the rejected records they list with criteria.Take; explain asks for 100 instead of
        // the search default of 20, so a slug shared by many stores still shows the records that matter.
        var resolver = new CriteriaRecordingResolver();
        var service = CreateService(resolver);

        await service.ExplainAsync(StoreId, null, Language, Language, Permalink);

        Assert.Equal(100, resolver.Criteria.Take);
    }

    [Fact]
    public async Task ExplainAsync_StagesKeepTheCandidatesOrder()
    {
        // An admin follows a record from stage to stage, so each stage lists the records in the Candidates order.
        // Ordered re-sorts by score, which is its purpose; with equal scores it keeps the order too.
        var resolver = new ExplainingResolver(
            resolved: [CreateSeoInfo("seo-1"), CreateSeoInfo("seo-3")],
            candidates: [CreateCandidate("seo-1"), CreateCandidate("seo-2", "NotInStoreCatalog"), CreateCandidate("seo-3")]);
        var service = CreateService(resolver);

        var result = await service.ExplainAsync(StoreId, null, Language, Language, Permalink);

        Assert.Equal(["seo-1", "seo-2", "seo-3"], result[0].Items.Select(x => x.SeoInfo.Id));
        Assert.All(result.Skip(1).SkipLast(1), x => Assert.Equal(["seo-1", "seo-3"], x.Items.Select(item => item.SeoInfo.Id)));
    }

    [Fact]
    public async Task ExplainAsync_NothingResolved_DoesNotPublishSeoInfoNotFoundEvent()
    {
        // Explain is a diagnostic GET: it must not register a broken link, which CompositeSeoResolver.FindSeoAsync
        // does through SeoInfoNotFoundEvent when nothing resolves.
        var eventPublisher = new EventPublisher();
        var resolver = new ExplainingResolver(resolved: [], candidates: [CreateCandidate("seo-1", "NotInStoreCatalog")]);
        var service = new SeoExplainService(new CompositeSeoResolver([resolver], eventPublisher));

        await service.ExplainAsync(StoreId, null, Language, Language, Permalink);

        Assert.Empty(eventPublisher.Events);
    }

    private static SeoExplainService CreateService(ISeoResolver resolver)
    {
        return new SeoExplainService(new CompositeSeoResolver([resolver], new EventPublisher()));
    }

    private static SeoInfo CreateSeoInfo(string id)
    {
        return new SeoInfo
        {
            Id = id,
            ObjectId = $"object-{id}",
            ObjectType = "Category",
            SemanticUrl = Permalink,
            StoreId = StoreId,
            LanguageCode = Language,
            IsActive = true,
        };
    }

    private static SeoExplainItem CreateCandidate(string id, params string[] reasonCodes)
    {
        var candidate = new SeoExplainItem(CreateSeoInfo(id));

        foreach (var reasonCode in reasonCodes)
        {
            candidate.Reasons.Add(new SeoCandidateReason(reasonCode));
        }

        return candidate;
    }

    /// <summary>
    /// A resolver that relies on the default ISeoResolver.GetCandidatesAsync.
    /// </summary>
    private sealed class Resolver(IList<SeoInfo> resolved) : ISeoResolver
    {
        public Task<IList<SeoInfo>> FindSeoAsync(SeoSearchCriteria criteria) => Task.FromResult(resolved);
    }

    /// <summary>
    /// A resolver that explains itself, as CatalogSeoResolver does.
    /// </summary>
    private sealed class ExplainingResolver(IList<SeoInfo> resolved, IList<SeoExplainItem> candidates) : ISeoResolver
    {
        public Task<IList<SeoInfo>> FindSeoAsync(SeoSearchCriteria criteria) => Task.FromResult(resolved);

        public Task<IList<SeoExplainItem>> GetCandidatesAsync(SeoSearchCriteria criteria) => Task.FromResult(candidates);
    }

    /// <summary>
    /// A resolver that records the criteria explain passes to it.
    /// </summary>
    private sealed class CriteriaRecordingResolver : ISeoResolver
    {
        public SeoSearchCriteria Criteria { get; private set; }

        public Task<IList<SeoInfo>> FindSeoAsync(SeoSearchCriteria criteria)
        {
            Criteria = criteria;
            return Task.FromResult<IList<SeoInfo>>([]);
        }
    }

    /// <summary>
    /// A project's own composite resolver written before GetCandidatesAsync existed.
    /// </summary>
    private sealed class CustomCompositeResolver(IList<SeoInfo> resolved) : ICompositeSeoResolver
    {
        public Task<IList<SeoInfo>> FindSeoAsync(SeoSearchCriteria criteria) => Task.FromResult(resolved);
    }

    private sealed class EventPublisher : IEventPublisher
    {
        public List<IEvent> Events { get; } = [];

        public Task Publish<T>(T @event, CancellationToken cancellationToken = default) where T : IEvent
        {
            Events.Add(@event);
            return Task.CompletedTask;
        }
    }
}
