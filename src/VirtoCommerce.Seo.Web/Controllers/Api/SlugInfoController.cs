using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Seo.Core;
using VirtoCommerce.Seo.Core.Models.SlugInfo;
using VirtoCommerce.Seo.Core.Services;

namespace VirtoCommerce.Seo.Web.Controllers.Api;

[Authorize]
[Route("api/seo/slug-info")]
public class SlugInfoController(ISlugInfoService slugInfoService) : Controller
{
    /// <summary>
    /// Find all SEO records for test by StoreId, StoreDefaultLanguage, LanguageCode, Permalink
    /// </summary>
    [HttpGet("explain")]
    [Authorize(ModuleConstants.Security.Permissions.Read)]
    [ProducesResponseType(typeof(SlugInfoResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<SlugInfoResponse>> GetExplainAsync(
        [FromQuery] string storeId,
        [FromQuery] string storeDefaultLanguage,
        [FromQuery] string languageCode,
        [FromQuery] string permalink)
    {
        var response = await slugInfoService.GetExplainAsync(storeId, storeDefaultLanguage, languageCode, permalink);

        return Ok(response);
    }
}
