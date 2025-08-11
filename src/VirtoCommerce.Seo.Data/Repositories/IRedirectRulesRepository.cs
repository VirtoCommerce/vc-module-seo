using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Seo.Data.Models;

namespace VirtoCommerce.Seo.Data.Repositories;

public interface IRedirectRulesRepository : IRepository
{
    IQueryable<RedirectRuleEntity> RedirectRules { get; }
    Task<IList<RedirectRuleEntity>> GetRedirectRulesByIdsAsync(IList<string> ids, string responseGroup);
}
