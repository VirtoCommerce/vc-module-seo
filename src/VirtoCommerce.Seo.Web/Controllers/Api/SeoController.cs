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
[Route("api/seoinfos")]
public class SeoController(
    ISeoDuplicatesDetector seoDuplicatesDetector,
    ICompositeSeoResolver compositeSeoResolver
    ) : Controller
{
    /// <summary>
    /// Batch create or update seo infos
    /// </summary>
    /// <param name="seoInfos"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("batchupdate")]
    public Task<ActionResult> BatchUpdateSeoInfos([FromBody] SeoInfo[] seoInfos)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Find all SEO records for object by objectId and objectType
    /// </summary>
    /// <param name="objectId"></param>
    /// <param name="objectType"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("duplicates")]
    public async Task<ActionResult<SeoInfo[]>> GetSeoDuplicates([FromQuery] string objectId, [FromQuery] string objectType)
    {
        var result = await seoDuplicatesDetector.DetectSeoDuplicatesAsync(new TenantIdentity(objectId, objectType));

        return Ok(result.ToArray());
    }

    /// <summary>
    /// Find all SEO records for object by slug 
    /// </summary>
    /// <param name="slug">slug</param>
    [HttpGet]
    [Route("{slug}")]
    public async Task<ActionResult<SeoInfo[]>> GetSeoInfoBySlug(string slug)
    {
        var criteria = new SeoSearchCriteria
        {
            Slug = slug,
            Take = 100,
        };
        var retVal = await compositeSeoResolver.FindSeoAsync(criteria);
        return Ok(retVal.ToArray());
    }

    /// <summary>
    /// Search SEO records based on criteria
    /// </summary>
    /// <param name="criteria"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("search")]
    public async Task<ActionResult<SeoInfo[]>> SearchSeo([FromBody] SeoSearchCriteria criteria)
    {
        var result = await compositeSeoResolver.FindSeoAsync(criteria);
        return Ok(result);
    }
}
