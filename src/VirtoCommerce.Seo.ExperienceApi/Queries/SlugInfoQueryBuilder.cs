using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.Seo.ExperienceApi.Models;
using VirtoCommerce.Xapi.Core.BaseQueries;
using SlugInfoResponseType = VirtoCommerce.Seo.ExperienceApi.Schemas.SlugInfoResponseType;

namespace VirtoCommerce.Seo.ExperienceApi.Queries;

public class SlugInfoQueryBuilder(IMediator mediator, IAuthorizationService authorizationService)
    : QueryBuilder<SlugInfoQuery, SlugInfoResponse, SlugInfoResponseType>(mediator, authorizationService)
{
    protected override string Name => "slugInfo";
}
