using System.Threading.Tasks;
using VirtoCommerce.Seo.Core.Models.Explain;

namespace VirtoCommerce.Seo.Core.Services;

public interface ISeoInfoExplainService
{
    public Task<SeoInfoExplainResponse> GetSeoInfoExplainAsync(string storeId, string storeDefaultLanguage, string languageCode, string permalink);
}
