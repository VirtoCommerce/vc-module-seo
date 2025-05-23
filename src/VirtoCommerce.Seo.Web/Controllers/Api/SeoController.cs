using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Services;
using SeoInfo = VirtoCommerce.Seo.Core.Models.SeoInfo;

namespace VirtoCommerce.Seo.Web.Controllers.Api;

[Authorize]
[Route("api")]
public class SeoController(
    ISeoDuplicatesDetector seoDuplicateDetector,
    CompositeSeoResolver seoResolverDecorator
    ) : Controller
{
    /// <summary>
    /// Batch create or update seo infos
    /// </summary>
    /// <param name="seoInfos"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("seoinfos/batchupdate")]
    public Task<ActionResult> BatchUpdateSeoInfos([FromBody] SeoInfo[] seoInfos)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [Route("seoinfos/duplicates")]
    public async Task<ActionResult<SeoInfo[]>> GetSeoDuplicates([FromQuery] string objectId, [FromQuery] string objectType)
    {
        var result = await seoDuplicateDetector.DetectSeoDuplicatesAsync(new TenantIdentity(objectId, objectType));

        return Ok(result.ToArray());
    }

    /// <summary>
    /// Find all SEO records for object by slug 
    /// </summary>
    /// <param name="slug">slug</param>
    [HttpGet]
    [Route("seoinfos/{slug}")]
    public async Task<ActionResult<SeoInfo[]>> GetSeoInfoBySlug(string slug)
    {
        var criteria = new SeoSearchCriteria
        {
            Slug = slug,
            Take = 100
        };
        var retVal = await seoResolverDecorator.FindSeoAsync(criteria);
        return Ok(retVal.ToArray());
    }
}
