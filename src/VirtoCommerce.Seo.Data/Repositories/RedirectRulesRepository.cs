using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Seo.Data.Models;

namespace VirtoCommerce.Seo.Data.Repositories;

public class RedirectRulesRepository(SeoDbContext dbContext, IUnitOfWork unitOfWork = null)
    : DbContextRepositoryBase<SeoDbContext>(dbContext, unitOfWork), IRedirectRulesRepository
{
    public IQueryable<RedirectRuleEntity> RedirectRules => DbContext.Set<RedirectRuleEntity>();

    public async Task<IList<RedirectRuleEntity>> GetRedirectRulesByIdsAsync(IList<string> ids, string responseGroup)
    {
        if (ids.IsNullOrEmpty())
        {
            return [];
        }

        return ids.Count == 1
            ? await RedirectRules.Where(x => x.Id == ids.First()).ToListAsync()
            : await RedirectRules.Where(x => ids.Contains(x.Id)).ToListAsync();

    }
}
