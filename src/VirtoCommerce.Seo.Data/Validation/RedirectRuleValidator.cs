using FluentValidation;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Seo.Core;
using VirtoCommerce.Seo.Core.Models;

namespace VirtoCommerce.Seo.Data.Validation;

public class RedirectRuleValidator : AbstractValidator<RedirectRule>
{
    public RedirectRuleValidator()
    {
        RuleFor(x => x.Inbound)
            .NotEmpty()
            .WithMessage("Inbound URL must not be empty.")
            .Must(BeAValidRegex)
            .WithMessage("Inbound must be a valid regular expression.");

        RuleFor(x => x.Outbound)
            .NotEmpty()
            .WithMessage("Outbound URL must not be empty.")
            .Must(BeAValidReplaceNumbers)
            .WithMessage("Outbound must contain valid replace numbers (e.g., $1, $2, ...) and not exceed inbound group count.");

        RuleFor(x => x.StoreId)
            .NotEmpty()
            .WithMessage("Store ID must not be empty.");
    }

    private bool BeAValidRegex(RedirectRule rule, string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
        {
            return false;
        }

        if (rule.RedirectRuleType == ModuleConstants.RedirectRuleType.Static)
        {
            return true;
        }

        try
        {
            _ = new System.Text.RegularExpressions.Regex(pattern);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private bool BeAValidReplaceNumbers(RedirectRule rule, string outbound)
    {
        if (outbound.IsNullOrWhiteSpace() || rule.Inbound.IsNullOrWhiteSpace())
        {
            return false;
        }

        // Count capturing groups in inbound regex
        int groupCount;
        try
        {
            var regex = new System.Text.RegularExpressions.Regex(rule.Inbound);
            groupCount = regex.GetGroupNumbers().Length - 1; // Exclude group 0 (entire match)
        }
        catch
        {
            return false;
        }

        // Find all $N in outbound
        var matches = System.Text.RegularExpressions.Regex.Matches(outbound, @"\$(\d+)");
        foreach (System.Text.RegularExpressions.Match match in matches)
        {
            if (!int.TryParse(match.Groups[1].Value, out int n) || n < 1 || n > groupCount)
            {
                return false;
            }
        }
        return true;
    }
}
