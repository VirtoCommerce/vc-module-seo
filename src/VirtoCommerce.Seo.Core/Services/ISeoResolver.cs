using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.Seo.Core.Models;

namespace VirtoCommerce.Seo.Core.Services;

public interface ISeoResolver
{
    Task<IList<SeoInfo>> FindSeoAsync(SeoSearchCriteria criteria);
}
