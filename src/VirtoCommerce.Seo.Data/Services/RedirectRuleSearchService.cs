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

public class RedirectRuleSearchService(
    Func<IRedirectRulesRepository> repositoryFactory,
    IPlatformMemoryCache platformMemoryCache,
    IRedirectRuleService crudService,
    IOptions<CrudOptions> crudOptions)
    : SearchService<RedirectRuleSearchCriteria, RedirectRuleSearchResult, RedirectRule, RedirectRuleEntity>
        (repositoryFactory, platformMemoryCache, crudService, crudOptions),
        IRedirectRuleSearchService
{
    protected override IQueryable<RedirectRuleEntity> BuildQuery(IRepository repository, RedirectRuleSearchCriteria criteria)
    {
        var query = ((IRedirectRulesRepository)repository).RedirectRules;

        if (!string.IsNullOrEmpty(criteria.Keyword))
        {
            query = query.Where(x => x.Inbound.Contains(criteria.Keyword) || x.Outbound.Contains(criteria.Keyword));
        }

        if (!string.IsNullOrEmpty(criteria.StoreId))
        {
            query = query.Where(x => x.StoreId == criteria.StoreId);
        }

        if (criteria.IsActive)
        {
            query = query.Where(x => x.IsActive);
        }

        return query;
    }

    protected override IList<SortInfo> BuildSortExpression(RedirectRuleSearchCriteria criteria)
    {
        var sortInfos = criteria.SortInfos;

        if (sortInfos.IsNullOrEmpty())
        {
            sortInfos =
            [
                new SortInfo { SortColumn = nameof(RedirectRuleEntity.CreatedDate), SortDirection = SortDirection.Descending },
                new SortInfo { SortColumn = nameof(RedirectRuleEntity.Id) },
            ];
        }

        return sortInfos;
    }
}
