using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Seo.Core;
using VirtoCommerce.Seo.Core.Events;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Services;

namespace VirtoCommerce.Seo.Data.Handlers;

public class SeoInfoNotFoundEventHandler(
    IBrokenLinkSearchService brokenLinkSearchService,
    IBrokenLinkService brokenLinkService)
    : IEventHandler<SeoInfoNotFoundEvent>
{
    public Task Handle(SeoInfoNotFoundEvent message)
    {
        ArgumentNullException.ThrowIfNull(message.Criteria);

        return HandleInternal(message.Criteria);
    }

    private async Task HandleInternal(SeoSearchCriteria criteria)
    {
        var searchCriteria = AbstractTypeFactory<BrokenLinkSearchCriteria>.TryCreateInstance();
        searchCriteria.StoreId = criteria.StoreId;
        searchCriteria.Permalink = criteria.Permalink;
        searchCriteria.LanguageCode = criteria.LanguageCode;
        searchCriteria.Take = 1;

        var searchResult = await brokenLinkSearchService.SearchNoCloneAsync(searchCriteria);

        var model = searchResult.Results.FirstOrDefault();

        if (model != null && model.Status != ModuleConstants.LinkStatus.Active)
        {
            return;
        }

        if (model == null)
        {
            model = AbstractTypeFactory<BrokenLink>.TryCreateInstance();

            model.StoreId = criteria.StoreId;
            model.Permalink = criteria.Permalink;
            model.Language = criteria.LanguageCode;
            model.Status = ModuleConstants.LinkStatus.Active;
            model.CreatedDate = DateTime.UtcNow;
        }

        model.Count++;
        model.LastAttemptTimestamp = DateTime.UtcNow;

        await brokenLinkService.SaveChangesAsync([model]);
    }
}
