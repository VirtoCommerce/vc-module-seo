using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Seo.Core;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Services;
using VirtoCommerce.Seo.Data.Models;

namespace VirtoCommerce.Seo.Data.Services;

public class RedirectResolver(IRedirectRuleSearchService redirectRuleSearchService) : IRedirectResolver
{
    public async Task<string> ResolveRedirect(string storeId, string url)
    {
        if (string.IsNullOrEmpty(storeId) || string.IsNullOrEmpty(url))
        {
            return null;
        }
        var criteria = AbstractTypeFactory<RedirectRuleSearchCriteria>.TryCreateInstance();
        criteria.IsActive = true;
        criteria.StoreId = storeId;
        criteria.SortInfos.AddRange(
            [
                new SortInfo { SortColumn = nameof(RedirectRuleEntity.Priority), SortDirection = SortDirection.Descending },
                new SortInfo { SortColumn = nameof(BrokenLinkEntity.Id) },
            ]
        );
        var rules = await redirectRuleSearchService.SearchAllNoCloneAsync(criteria);
        foreach (var rule in rules)
        {
            if (rule.RedirectRuleType == ModuleConstants.RedirectRuleType.Static)
            {
                if (url.Equals(rule.Inbound, StringComparison.OrdinalIgnoreCase))
                {
                    return rule.Outbound;
                }
                continue;
            }

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
