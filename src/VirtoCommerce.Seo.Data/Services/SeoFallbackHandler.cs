using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Seo.Core;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Services;

namespace VirtoCommerce.Seo.Data.Services;

public class SeoFallbackHandler(
    IBrokenLinkSearchService brokenLinkSearchService, IBrokenLinkService brokenLinkService
) : ISeoFallbackHandler
{
    public async Task HandleFallback(SeoSearchCriteria criteria)
    {
        if (criteria == null)
        {
            throw new ArgumentNullException(nameof(criteria));
        }

        var brokenListCriteria = AbstractTypeFactory<BrokenLinkSearchCriteria>.TryCreateInstance();
        brokenListCriteria.Permalink = criteria.Permalink;
        brokenListCriteria.StoreId = criteria.StoreId;
        brokenListCriteria.LanguageCode = criteria.LanguageCode;

        var models = await brokenLinkSearchService.SearchNoCloneAsync(brokenListCriteria);

        var model = models.Results.FirstOrDefault();

        if (model != null && model.Status != ModuleConstants.LinkStatus.Active)
        {
            return;
        }

        if (model == null)
        {
            model = AbstractTypeFactory<BrokenLink>.TryCreateInstance();

            model.Permalink = criteria.Permalink;
            model.StoreId = criteria.StoreId;
            model.Language = criteria.LanguageCode;
            model.Status = ModuleConstants.LinkStatus.Active;
            model.CreatedDate = DateTime.UtcNow;
        }

        model.Count++;
        model.LastAttemptTimestamp = DateTime.UtcNow;

        await brokenLinkService.SaveChangesAsync([model]);
    }
}
