using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Seo.Core.Models;

public class BrokenLink : AuditableEntity, ICloneable
{
    public int Count { get; set; }

    public string Permalink { get; set; }

    [StringLength(64)]
    public string StoreId { get; set; }

    [StringLength(64)]
    public string Status { get; set; }

    public string Language { get; set; }

    public string RedirectUrl { get; set; }

    public DateTime LastAttemptTimestamp { get; set; }

    public object Clone()
    {
        return MemberwiseClone();
    }
}
