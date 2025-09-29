using System.Collections.Generic;

namespace VirtoCommerce.Seo.Core.Models.SlugInfo;

public sealed record SeoInfosResponse
{
    public PipelineStage Stage { get; init; }
    public string Description { get; init; }
    public IReadOnlyList<SeoInfoResponse> SeoInfoResponses { get; init; }

    public SeoInfosResponse(string description, IReadOnlyList<SeoInfoResponse> seoInfoResponses)
        : this(PipelineStage.Unknown, description, seoInfoResponses)
    {
    }

    public SeoInfosResponse(PipelineStage stage, string description, IReadOnlyList<SeoInfoResponse> seoInfoResponses)
    {
        Stage = stage;
        Description = description;
        SeoInfoResponses = seoInfoResponses ?? new List<SeoInfoResponse>();
    }
}
