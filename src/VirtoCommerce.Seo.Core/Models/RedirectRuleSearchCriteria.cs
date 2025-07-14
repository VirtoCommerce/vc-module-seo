using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Seo.Core.Models;

public class RedirectRuleSearchCriteria : SearchCriteriaBase
{
    public bool IsActive { get; set; }
    public string StoreId { get; set; }
}
