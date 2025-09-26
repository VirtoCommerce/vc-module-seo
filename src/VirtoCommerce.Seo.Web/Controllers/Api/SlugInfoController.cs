using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Seo.Core;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Services;
using VirtoCommerce.Seo.Data.Services;

namespace VirtoCommerce.Seo.Web.Controllers.Api;

[Authorize]
[Route("api/seo/slug-info")]
public class SlugInfoController(ISlugInfoService slugInfoService) : Controller
{
    /// <summary>
    /// Find all SEO records for test by StoreId, LanguageCode, Permalink
    /// </summary>
    [HttpGet("explain")]
    [Authorize(ModuleConstants.Security.Permissions.Read)]
    public async Task<ActionResult<SeoForTestResponse>> GetExplainAsync(SeoForTestRequest request)
    {
        var storeId = request.StoreId;
        var languageCode = request.LanguageCode;
        var permalink = request.Permalink;

        var processOrder = await slugInfoService.GetSeoInfoForTestAsync(storeId, languageCode, permalink);

        var response = new SeoForTestResponse(storeId, languageCode, permalink, processOrder);

        return StatusCode(StatusCodes.Status200OK, response);
    }
}
