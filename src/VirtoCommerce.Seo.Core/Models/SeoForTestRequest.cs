using Microsoft.AspNetCore.Mvc;

namespace VirtoCommerce.Seo.Core.Models;

public class SeoForTestRequest
{
    [FromQuery]
    public string StoreId { get; set; }

    [FromQuery]
    public string LanguageCode { get; set; }

    [FromQuery]
    public string Permalink { get; set; }
}
