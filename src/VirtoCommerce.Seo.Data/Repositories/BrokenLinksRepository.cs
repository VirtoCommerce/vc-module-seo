using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Seo.Data.Models;

namespace VirtoCommerce.Seo.Data.Repositories;

public class BrokenLinksRepository(SeoDbContext dbContext, IUnitOfWork unitOfWork = null)
    : DbContextRepositoryBase<SeoDbContext>(dbContext, unitOfWork), IBrokenLinksRepository
{
    public IQueryable<BrokenLinkEntity> BrokenLinks => DbContext.Set<BrokenLinkEntity>();

    public async Task<IList<BrokenLinkEntity>> GetBrokenLinksByIdsAsync(IList<string> ids, string responseGroup)
    {
        if (ids.IsNullOrEmpty())
        {
            return [];
        }

        return ids.Count == 1
            ? await BrokenLinks.Where(x => x.Id == ids.First()).ToListAsync()
            : await BrokenLinks.Where(x => ids.Contains(x.Id)).ToListAsync();

    }
}
