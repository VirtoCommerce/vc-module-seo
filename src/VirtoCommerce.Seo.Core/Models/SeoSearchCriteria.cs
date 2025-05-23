using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Seo.Core.Models;

public class SeoSearchCriteria : SearchCriteriaBase
{
    public string UserId { get; set; }
    public string Slug { get; set; }
    public string StoreId { get; set; }
    public string Permalink { get; set; }
}
