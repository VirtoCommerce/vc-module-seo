using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Seo.Core.Models;

namespace VirtoCommerce.Seo.Core.Services;

public interface IBrokenLinkSearchService : ISearchService<BrokenLinkSearchCriteria, BrokenLinkSearchResult, BrokenLink>;
