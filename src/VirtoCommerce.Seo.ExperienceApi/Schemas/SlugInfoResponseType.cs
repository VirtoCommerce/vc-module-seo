using VirtoCommerce.Seo.ExperienceApi.Models;
using VirtoCommerce.Xapi.Core.Schemas;

namespace VirtoCommerce.Seo.ExperienceApi.Schemas;

public class SlugInfoResponseType : ExtendableGraphType<SlugInfoResponse>
{
    public SlugInfoResponseType()
    {
        Field<SeoInfoType>("entityInfo").Description("SEO info").Resolve(context => context.Source.EntityInfo);
    }
}
