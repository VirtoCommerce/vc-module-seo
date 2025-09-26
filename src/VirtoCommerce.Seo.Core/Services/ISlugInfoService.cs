using System.Threading.Tasks;
using VirtoCommerce.Seo.Core.Models;

namespace VirtoCommerce.Seo.Core.Services;

public interface ISlugInfoService
{
    public Task<ProcessOrderSeoInfoResponse> GetSeoInfoForTestAsync(string storeId, string languageCode, string permalink, string storeDefaultLanguage = "en-US");
}
