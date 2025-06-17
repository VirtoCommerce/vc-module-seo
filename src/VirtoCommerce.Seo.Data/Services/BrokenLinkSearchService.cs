using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Data.GenericCrud;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Services;
using VirtoCommerce.Seo.Data.Models;
using VirtoCommerce.Seo.Data.Repositories;

namespace VirtoCommerce.Seo.Data.Services;

public class BrokenLinkSearchService(
    Func<IBrokenLinksRepository> repositoryFactory,
    IPlatformMemoryCache platformMemoryCache,
    IBrokenLinkService crudService,
    IOptions<CrudOptions> crudOptions)
    : SearchService<BrokenLinkSearchCriteria, BrokenLinkSearchResult, BrokenLink, BrokenLinkEntity>(repositoryFactory,
        platformMemoryCache, crudService, crudOptions), IBrokenLinkSearchService
{
    protected override IQueryable<BrokenLinkEntity> BuildQuery(IRepository repository, BrokenLinkSearchCriteria criteria)
    {
        var query = ((IBrokenLinksRepository)repository).BrokenLinks;

        if (!string.IsNullOrEmpty(criteria.Keyword))
        {
            query = query.Where(x => x.Permalink.Contains(criteria.Keyword));
        }

        if (!string.IsNullOrEmpty(criteria.Permalink))
        {
            query = query.Where(x => x.Permalink == criteria.Permalink);
        }

        if (!string.IsNullOrEmpty(criteria.StoreId))
        {
            query = query.Where(x => x.StoreId == criteria.StoreId);
        }

        if (!string.IsNullOrEmpty(criteria.Status))
        {
            query = query.Where(x => x.Status == criteria.Status);
        }

        if (!string.IsNullOrEmpty(criteria.LanguageCode))
        {
            query = query.Where(x => x.LanguageCode == criteria.LanguageCode || string.IsNullOrEmpty(x.LanguageCode));
        }

        return query;
    }

    protected override IList<SortInfo> BuildSortExpression(BrokenLinkSearchCriteria criteria)
    {
        var sortInfos = criteria.SortInfos;

        if (sortInfos.IsNullOrEmpty())
        {
            sortInfos =
            [
                new SortInfo { SortColumn = nameof(BrokenLinkEntity.CreatedDate), SortDirection = SortDirection.Descending },
                new SortInfo { SortColumn = nameof(BrokenLinkEntity.Id) },
            ];
        }

        return sortInfos;
    }
}
