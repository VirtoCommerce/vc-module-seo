using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Seo.Core.Models;

namespace VirtoCommerce.Seo.Core.Events;

public class SeoInfoNotFoundEvent : DomainEvent
{
    public SeoSearchCriteria Criteria { get; set; }
}
