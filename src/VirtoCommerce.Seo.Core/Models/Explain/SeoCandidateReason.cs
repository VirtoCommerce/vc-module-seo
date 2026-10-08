namespace VirtoCommerce.Seo.Core.Models.Explain;

public class SeoCandidateReason(string code, string details = null)
{
    public string Code { get; } = code;

    public string Details { get; } = details;
}
