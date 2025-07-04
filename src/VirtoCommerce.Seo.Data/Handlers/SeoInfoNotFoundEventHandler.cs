using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Seo.Core;
using VirtoCommerce.Seo.Core.Events;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Services;

namespace VirtoCommerce.Seo.Data.Handlers;

public class SeoInfoNotFoundEventHandler(
    IBrokenLinkSearchService brokenLinkSearchService,
    IBrokenLinkService brokenLinkService,
    ISettingsManager settingsManager)
    : IEventHandler<SeoInfoNotFoundEvent>
{
    public Task Handle(SeoInfoNotFoundEvent message)
    {
        ArgumentNullException.ThrowIfNull(message.Criteria);

        return HandleInternal(message.Criteria);
    }

    private async Task HandleInternal(SeoSearchCriteria criteria)
    {
        var brokenLinkDetectionEnabled = await settingsManager.GetValueAsync<bool>(ModuleConstants.Settings.General.BrokenLinkDetectionEnabled);
        if (!brokenLinkDetectionEnabled)
        {
            return;
        }

        var searchCriteria = AbstractTypeFactory<BrokenLinkSearchCriteria>.TryCreateInstance();
        searchCriteria.Permalink = criteria.Permalink;
        searchCriteria.StoreId = criteria.StoreId;
        searchCriteria.LanguageCode = criteria.LanguageCode;
        searchCriteria.Take = 1;

        var searchResult = await brokenLinkSearchService.SearchNoCloneAsync(searchCriteria);

        var model = searchResult.Results.FirstOrDefault();

        if (model != null && model.Status != ModuleConstants.LinkStatus.Active)
        {
            return;
        }

        BackgroundJob.Enqueue(() => SaveBrokenLink(model, criteria));
    }

    public Task SaveBrokenLink(BrokenLink model, SeoSearchCriteria criteria)
    {
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
