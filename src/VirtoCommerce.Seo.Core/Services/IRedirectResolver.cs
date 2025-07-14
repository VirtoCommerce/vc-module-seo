using System.Threading.Tasks;

namespace VirtoCommerce.Seo.Core.Services;

public interface IRedirectResolver
{
    Task<string> ResolveRedirect(string storeId, string url);
}
