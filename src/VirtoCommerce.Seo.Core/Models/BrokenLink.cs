using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Seo.Core.Models;

public class BrokenLink : AuditableEntity, ICloneable
{
    public int Count { get; set; }

    public string Permalink { get; set; }

    public string StoreId { get; set; }

    public string Status { get; set; }

    public string Language { get; set; }

    public string RedirectUrl { get; set; }

    public DateTime LastAttemptTimestamp { get; set; }

    public object Clone()
    {
        return MemberwiseClone();
    }
}
