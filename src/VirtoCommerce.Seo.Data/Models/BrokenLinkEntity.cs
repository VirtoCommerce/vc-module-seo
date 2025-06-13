using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Seo.Core.Models;

namespace VirtoCommerce.Seo.Data.Models;

public class BrokenLinkEntity : AuditableEntity, IDataEntity<BrokenLinkEntity, BrokenLink>
{
    public int Count { get; set; }

    [StringLength(2048)]
    public string Permalink { get; set; }

    [StringLength(64)]
    public string StoreId { get; set; }

    [StringLength(64)]
    public string Status { get; set; }

    [StringLength(2048)]
    public string RedirectUrl { get; set; }

    [StringLength(128)]
    public string LanguageCode { get; set; }

    public DateTime LastAttemptTimestamp { get; set; }

    public BrokenLink ToModel(BrokenLink model)
    {
        model.Id = Id;
        model.CreatedBy = CreatedBy;
        model.CreatedDate = CreatedDate;
        model.ModifiedBy = ModifiedBy;
        model.ModifiedDate = ModifiedDate;
        model.Language = LanguageCode;

        model.Count = Count;
        model.Permalink = Permalink;
        model.StoreId = StoreId;
        model.Status = Status;
        model.RedirectUrl = RedirectUrl;
        model.LastAttemptTimestamp = LastAttemptTimestamp;

        return model;
    }

    public BrokenLinkEntity FromModel(BrokenLink model, PrimaryKeyResolvingMap pkMap)
    {
        pkMap.AddPair(model, this);

        Id = model.Id;
        CreatedBy = model.CreatedBy;
        CreatedDate = model.CreatedDate;
        ModifiedBy = model.ModifiedBy;
        ModifiedDate = model.ModifiedDate;
        LanguageCode = model.Language;

        Count = model.Count;
        Permalink = model.Permalink;
        StoreId = model.StoreId;
        Status = model.Status;
        RedirectUrl = model.RedirectUrl;
        LastAttemptTimestamp = model.LastAttemptTimestamp;

        return this;
    }

    public void Patch(BrokenLinkEntity target)
    {
        target.Count = Count;
        target.Permalink = Permalink;
        target.StoreId = StoreId;
        target.Status = Status;
        target.RedirectUrl = RedirectUrl;
    }
}
