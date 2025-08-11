using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Seo.Core.Models;

namespace VirtoCommerce.Seo.Core.Events;

public class RedirectRuleChangedEvent(IEnumerable<GenericChangedEntry<RedirectRule>> changedEntries)
    : GenericChangedEntryEvent<RedirectRule>(changedEntries);
