using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Seo.Core;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Data.Services;

namespace VirtoCommerce.Seo.Web.Controllers.Api;

[Authorize]
[Route("api/maintenance")]
public class MaintenanceController(IMaintenanceService maintenanceService) : Controller
{
    /// <summary>
    /// Find all SEO records for test by StoreId, LanguageCode, Permalink
    /// </summary>
    [HttpGet("seo-for-test-mode")]
    [Authorize(ModuleConstants.Security.Permissions.Read)]
    public async Task<ActionResult<SeoForTestResponse>> GetSeoInfoForTestAsync(SeoForTestRequest request)
    {
        var storeId = request.StoreId;
        var languageCode = request.LanguageCode;
        var permalink = request.Permalink;

        var processOrder = await maintenanceService.GetSeoInfoForTestAsync(storeId, languageCode, permalink, HttpContext.RequestAborted);

        var response = new SeoForTestResponse(storeId, languageCode, permalink, processOrder);

        return StatusCode(StatusCodes.Status200OK, response);
    }
}
