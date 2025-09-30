using System.Threading.Tasks;
using VirtoCommerce.Seo.Core.Models.Explain;

namespace VirtoCommerce.Seo.Core.Services;

public interface ISeoExplainService
{
    public Task<SeoExplainResponse> GetSeoExplainAsync(string storeId, string storeDefaultLanguage, string languageCode, string permalink);
}
