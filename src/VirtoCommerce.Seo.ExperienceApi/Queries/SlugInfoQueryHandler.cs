using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Seo.Core.Extensions;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Services;
using VirtoCommerce.Seo.ExperienceApi.Models;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.Xapi.Core.Infrastructure;

namespace VirtoCommerce.Seo.ExperienceApi.Queries;

public class SlugInfoQueryHandler(CompositeSeoResolver seoResolver, IStoreService storeService)
    : IQueryHandler<SlugInfoQuery, SlugInfoResponse>
{
    public async Task<SlugInfoResponse> Handle(SlugInfoQuery request, CancellationToken cancellationToken)
    {
        var result = new SlugInfoResponse();

        if (string.IsNullOrEmpty(request.Permalink))
        {
            return result;
        }

        var store = await storeService.GetByIdAsync(request.StoreId);
        if (store is null)
        {
            return result;
        }

        // todo: is it correct? should it be just the cultureName?
        var currentCulture = request.CultureName ?? store.DefaultLanguage;

        var segments = request.Permalink.Split("/", StringSplitOptions.RemoveEmptyEntries);
        var lastSegment = segments.LastOrDefault();

        var criteria = AbstractTypeFactory<SeoSearchCriteria>.TryCreateInstance();
        criteria.StoreId = store.Id;
        criteria.LanguageCode = currentCulture;
        criteria.Permalink = request.Permalink;
        criteria.Slug = lastSegment;
        criteria.UserId = request.UserId;

        result.EntityInfo = await GetBestMatchingSeoInfo(criteria, criteria.StoreId, store.DefaultLanguage);

        return result;
    }

    protected virtual async Task<SeoInfo> GetBestMatchingSeoInfo(SeoSearchCriteria criteria, string storeId, string defaultLanguage)
    {
        var itemsToMatch = await seoResolver.FindSeoAsync(criteria);
        return itemsToMatch.GetBestMatchingSeoInfo(storeId, defaultLanguage, criteria.LanguageCode, criteria.Slug, criteria.Permalink);
    }
}
