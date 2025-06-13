using System.Threading.Tasks;
using VirtoCommerce.Seo.Core.Models;

namespace VirtoCommerce.Seo.Core.Services;

public interface ISeoFallbackHandler
{
    Task HandleFallback(SeoSearchCriteria criteria);
}
