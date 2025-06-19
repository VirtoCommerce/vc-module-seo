using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Seo.Core.Models;

namespace VirtoCommerce.Seo.Core.Events;

public class BrokenLinkChangedEvent(IEnumerable<GenericChangedEntry<BrokenLink>> changedEntries)
    : GenericChangedEntryEvent<BrokenLink>(changedEntries);
