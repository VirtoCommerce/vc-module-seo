using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Seo.Core.Models;

public class BrokenLinkSearchCriteria : SearchCriteriaBase
{
    public string StoreId { get; set; }
    public string Permalink { get; set; }
    public string Status { get; set; }
}
