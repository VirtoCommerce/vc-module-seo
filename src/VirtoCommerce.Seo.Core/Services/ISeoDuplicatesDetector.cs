using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Seo.Core.Models;

namespace VirtoCommerce.Seo.Core.Services;

/// <summary>
/// Used to detect SEO duplicates within any object based on its inner structure and relationships (store, catalogs, categories etc.)
/// </summary>
public interface ISeoDuplicatesDetector
{
    Task<IEnumerable<SeoInfo>> DetectSeoDuplicatesAsync(TenantIdentity tenantIdentity);
}
