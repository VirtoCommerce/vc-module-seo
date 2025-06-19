using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Seo.Core.Models;

namespace VirtoCommerce.Seo.Data.Models;

public class BrokenLinkEntity : AuditableEntity, IDataEntity<BrokenLinkEntity, BrokenLink>
{
    [StringLength(2048)]
    public string Permalink { get; set; }

    [StringLength(128)]
    public string StoreId { get; set; }

    [StringLength(128)]
    public string LanguageCode { get; set; }

    [StringLength(64)]
    public string Status { get; set; }

    [StringLength(2048)]
    public string RedirectUrl { get; set; }

    public int HitCount { get; set; }

    public DateTime LastHitDate { get; set; }

    public virtual BrokenLink ToModel(BrokenLink model)
    {
        model.Id = Id;
        model.CreatedBy = CreatedBy;
        model.CreatedDate = CreatedDate;
        model.ModifiedBy = ModifiedBy;
        model.ModifiedDate = ModifiedDate;

        model.Permalink = Permalink;
        model.StoreId = StoreId;
        model.Language = LanguageCode;
        model.Status = Status;
        model.RedirectUrl = RedirectUrl;
        model.HitCount = HitCount;
        model.LastHitDate = LastHitDate;

        return model;
    }

    public virtual BrokenLinkEntity FromModel(BrokenLink model, PrimaryKeyResolvingMap pkMap)
    {
        pkMap.AddPair(model, this);

        Id = model.Id;
        CreatedBy = model.CreatedBy;
        CreatedDate = model.CreatedDate;
        ModifiedBy = model.ModifiedBy;
        ModifiedDate = model.ModifiedDate;

        Permalink = model.Permalink;
        StoreId = model.StoreId;
        LanguageCode = model.Language;
        Status = model.Status;
        RedirectUrl = model.RedirectUrl;
        HitCount = model.HitCount;
        LastHitDate = model.LastHitDate;

        return this;
    }

    public virtual void Patch(BrokenLinkEntity target)
    {
        target.Permalink = Permalink;
        target.StoreId = StoreId;
        target.LanguageCode = LanguageCode;
        target.Status = Status;
        target.RedirectUrl = RedirectUrl;
        target.HitCount = HitCount;
        target.LastHitDate = LastHitDate;
    }
}
