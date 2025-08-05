using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Seo.Core.Models;

public class RedirectRule : AuditableEntity, ICloneable
{
    public bool IsActive { get; set; }
    public string Inbound { get; set; }
    public string Outbound { get; set; }
    public string StoreId { get; set; }
    public int Priority { get; set; }
    public string RedirectRuleType { get; set; }

    public object Clone()
    {
        return MemberwiseClone();
    }
}
