namespace VirtoCommerce.Seo.Core.Models;

public class SeoForTestResponse(string storeId, string languageCode, string permalink, ProcessOrderSeoInfoResponse processOrder)
{
    public string StoreId { get; set; } = storeId;
    public string LanguageCode { get; set; } = languageCode;
    public string Permalink { get; set; } = permalink;
    public ProcessOrderSeoInfoResponse ProcessOrder { get; set; } = processOrder;
}
