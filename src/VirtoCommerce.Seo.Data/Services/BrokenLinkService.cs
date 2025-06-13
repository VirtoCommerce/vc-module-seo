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

public class BrokenLinkService(
    Func<IBrokenLinksRepository> repositoryFactory,
    IPlatformMemoryCache platformMemoryCache,
    IEventPublisher eventPublisher)
    : CrudService<BrokenLink, BrokenLinkEntity, BrokenLinkChangingEvent, BrokenLinkChangedEvent>(repositoryFactory,
        platformMemoryCache, eventPublisher), IBrokenLinkService
{
    protected override Task<IList<BrokenLinkEntity>> LoadEntities(IRepository repository, IList<string> ids, string responseGroup)
    {
        return ((IBrokenLinksRepository)repository).GetBrokenLinksByIdsAsync(ids, responseGroup);
    }
}
