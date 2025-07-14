using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.GenericCrud;
using VirtoCommerce.Seo.Core.Events;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Services;
using VirtoCommerce.Seo.Data.Models;
using VirtoCommerce.Seo.Data.Repositories;

namespace VirtoCommerce.Seo.Data.Services;

public class RedirectRuleService(
    Func<IRedirectRulesRepository> repositoryFactory,
    IPlatformMemoryCache platformMemoryCache,
    IEventPublisher eventPublisher)
    : CrudService<RedirectRule, RedirectRuleEntity, RedirectRuleChangingEvent, RedirectRuleChangedEvent>
        (repositoryFactory, platformMemoryCache, eventPublisher),
        IRedirectRuleService
{
    protected override Task<IList<RedirectRuleEntity>> LoadEntities(IRepository repository, IList<string> ids, string responseGroup)
    {
        return ((IRedirectRulesRepository)repository).GetRedirectRulesByIdsAsync(ids, responseGroup);
    }
}
