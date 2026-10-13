using System;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Jobs;
using VirtoCommerce.Seo.Core;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Services;

namespace VirtoCommerce.Seo.Data.BackgroundJobs;

/// <summary>
/// Payload for <see cref="SaveBrokenLinkJob"/>: the broken-link record to upsert (may be null — a new one is
/// created from <see cref="Criteria"/>) and the originating search criteria.
/// </summary>
public class SaveBrokenLinkPayload
{
    public BrokenLink Model { get; set; }
    public SeoSearchCriteria Criteria { get; set; }
}

/// <summary>
/// Engine-agnostic background job that records/updates a broken link. Replaces the former
/// Hangfire <c>BackgroundJob.Enqueue(() =&gt; SaveBrokenLink(...))</c> call.
/// </summary>
public class SaveBrokenLinkJob(IBrokenLinkService brokenLinkService) : IBackgroundJobHandler<SaveBrokenLinkPayload>
{
    public Task Execute(SaveBrokenLinkPayload payload, IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        var model = payload.Model;
        var criteria = payload.Criteria;

        if (model == null)
        {
            model = AbstractTypeFactory<BrokenLink>.TryCreateInstance();

            model.Permalink = criteria.Permalink;
            model.StoreId = criteria.StoreId;
            model.Language = criteria.LanguageCode;
            model.Status = ModuleConstants.LinkStatus.Active;
            model.CreatedDate = DateTime.UtcNow;
        }

        model.HitCount++;
        model.LastHitDate = DateTime.UtcNow;

        return brokenLinkService.SaveChangesAsync([model]);
    }
}
