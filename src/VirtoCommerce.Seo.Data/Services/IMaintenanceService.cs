using System.Threading.Tasks;
using VirtoCommerce.Seo.Core.Models;

namespace VirtoCommerce.Seo.Data.Services;

public interface IMaintenanceService
{
    public Task<ProcessOrderSeoInfoResponse> GetSeoInfoForTestAsync(string storeId, string languageCode, string permalink, string storeDefaultLanguage = "en-US");
}
