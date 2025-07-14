using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Services;

namespace VirtoCommerce.Seo.Data.Services;

public class RedirectResolver(IRedirectRuleSearchService redirectRuleSearchService) : IRedirectResolver
{
    public async Task<string> ResolveRedirect(string storeId, string url)
    {
        var criteria = AbstractTypeFactory<RedirectRuleSearchCriteria>.TryCreateInstance();
        criteria.IsActive = true;
        criteria.StoreId = storeId;
        var rules = await redirectRuleSearchService.SearchAllNoCloneAsync(criteria);
        foreach (var rule in rules)
        {
            var regex = new Regex(rule.Inbound);
            var match = regex.Match(url);
            if (match.Success)
            {
                var result = rule.Outbound;
                for (var i = 1; i < match.Groups.Count; i++)
                {
                    result = result.Replace($"${i}", match.Groups[i].Value);
                }
                return result;
            }
        }
        return null;
    }
}
