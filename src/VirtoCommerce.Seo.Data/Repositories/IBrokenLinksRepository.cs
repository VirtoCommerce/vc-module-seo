using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Seo.Data.Models;

namespace VirtoCommerce.Seo.Data.Repositories;

public interface IBrokenLinksRepository : IRepository
{
    IQueryable<BrokenLinkEntity> BrokenLinks { get; }
    Task<IList<BrokenLinkEntity>> GetBrokenLinksByIdsAsync(IList<string> ids, string responseGroup);
}
