using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Seo.Core.Models;

namespace VirtoCommerce.Seo.Data.Models;

public class RedirectRuleEntity : AuditableEntity, IDataEntity<RedirectRuleEntity, RedirectRule>
{
    public bool IsActive { get; set; }

    [StringLength(2048)]
    public string Inbound { get; set; }

    [StringLength(2048)]
    public string Outbound { get; set; }

    [StringLength(128)]
    public string StoreId { get; set; }

    public int Priority { get; set; }

    [StringLength(128)]
    public string RedirectRuleType { get; set; }

    public RedirectRule ToModel(RedirectRule model)
    {
        model.Id = Id;
        model.CreatedBy = CreatedBy;
        model.CreatedDate = CreatedDate;
        model.ModifiedBy = ModifiedBy;
        model.ModifiedDate = ModifiedDate;

        model.IsActive = IsActive;
        model.Inbound = Inbound;
        model.Outbound = Outbound;
        model.StoreId = StoreId;
        model.Priority = Priority;
        model.RedirectRuleType = RedirectRuleType;

        return model;
    }

    public RedirectRuleEntity FromModel(RedirectRule model, PrimaryKeyResolvingMap pkMap)
    {
        pkMap.AddPair(model, this);

        Id = model.Id;
        CreatedBy = model.CreatedBy;
        CreatedDate = model.CreatedDate;
        ModifiedBy = model.ModifiedBy;
        ModifiedDate = model.ModifiedDate;

        IsActive = model.IsActive;
        Inbound = model.Inbound;
        Outbound = model.Outbound;
        StoreId = model.StoreId;
        Priority = model.Priority;
        RedirectRuleType = model.RedirectRuleType;

        return this;
    }

    public void Patch(RedirectRuleEntity target)
    {
        target.StoreId = StoreId;
        target.IsActive = IsActive;
        target.Inbound = Inbound;
        target.Outbound = Outbound;
        target.Priority = Priority;
        target.RedirectRuleType = RedirectRuleType;
    }
}
