using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Seo.Core.Models;

public class BrokenLink : AuditableEntity, ICloneable
{
    public string Permalink { get; set; }

    public string StoreId { get; set; }

    public string Language { get; set; }

    public string Status { get; set; }

    public string RedirectUrl { get; set; }

    public int HitCount { get; set; }

    public DateTime LastHitDate { get; set; }

    public object Clone()
    {
        return MemberwiseClone();
    }
}
