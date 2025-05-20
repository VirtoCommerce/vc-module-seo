using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Permissions = VirtoCommerce.Seo.Core.ModuleConstants.Security.Permissions;

namespace VirtoCommerce.Seo.Web.Controllers.Api;

[Authorize]
[Route("api/seo")]
public class SeoController : Controller
{
    // GET: api/seo
    /// <summary>
    /// Get message
    /// </summary>
    /// <remarks>Return "Hello world!" message</remarks>
    [HttpGet]
    [Route("")]
    [Authorize(Permissions.Read)]
    public ActionResult<string> Get()
    {
        return Ok(new { result = "Hello world!" });
    }
}
