using System.Threading.Tasks;
using VirtoCommerce.Seo.Core.Models.SlugInfo;

namespace VirtoCommerce.Seo.Core.Services;

public interface ISlugExplainService
{
    public Task<SlugInfoResponse> GetExplainAsync(string storeId, string storeDefaultLanguage, string languageCode, string permalink);
}
