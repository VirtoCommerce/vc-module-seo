using System.Text.RegularExpressions;
using FluentValidation;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Seo.Core;
using VirtoCommerce.Seo.Core.Models;

namespace VirtoCommerce.Seo.Data.Validation;

public partial class RedirectRuleValidator : AbstractValidator<RedirectRule>
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
            _ = new Regex(pattern);
            return true;
        }
        catch
        {
            return false;
        }
    }

    [GeneratedRegex(@"\$(\d+)")]
    private static partial Regex OutboundParametersGeneratedRegex();

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
            var regex = new Regex(rule.Inbound);
            groupCount = regex.GetGroupNumbers().Length - 1; // Exclude group 0 (entire match)
        }
        catch
        {
            return false;
        }

        var matches = OutboundParametersGeneratedRegex().Matches(outbound);
        foreach (Match match in matches)
        {
            if (!int.TryParse(match.Groups[1].Value, out var n) || n < 1 || n > groupCount)
            {
                return false;
            }
        }
        return true;
    }
}
