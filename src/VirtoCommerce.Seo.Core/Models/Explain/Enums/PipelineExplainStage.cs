namespace VirtoCommerce.Seo.Core.Models.Explain.Enums;

public enum PipelineExplainStage
{
    Unknown = 0,
    Original = 1,
    Filtered = 2,
    Scored = 3,
    FilteredScore = 4,
    Ordered = 5,
    Final = 6,
}
