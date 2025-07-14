using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Seo.Core;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Services;

namespace VirtoCommerce.Seo.Web.Controllers.Api;

[Authorize]
[Route("api/seo/broken-links")]
public class BrokenLinksController(
    IBrokenLinkSearchService brokenLinkSearchService,
    IBrokenLinkService brokenLinkService)
    : Controller
{
    [HttpGet]
    [Route("{id}")]
    [Authorize(ModuleConstants.Security.Permissions.Read)]
    public async Task<ActionResult<BrokenLink>> GetById(string id)
    {
        var contract = await brokenLinkService.GetNoCloneAsync(id);
        return Ok(contract);
    }

    [HttpPost]
    [Route("search")]
    [Authorize(ModuleConstants.Security.Permissions.Read)]
    public async Task<ActionResult<BrokenLinkSearchResult>> Search([FromBody] BrokenLinkSearchCriteria criteria)
    {
        var result = await brokenLinkSearchService.SearchNoCloneAsync(criteria);
        return Ok(result);
    }

    [HttpPost]
    [Route("")]
    [Authorize(ModuleConstants.Security.Permissions.Create)]
    public async Task<ActionResult<BrokenLink>> Create([FromBody] BrokenLink model)
    {
        await brokenLinkService.SaveChangesAsync([model]);
        return Ok(model);
    }

    [HttpPut]
    [Route("")]
    [Authorize(ModuleConstants.Security.Permissions.Update)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<ActionResult<BrokenLink>> Update([FromBody] BrokenLink model)
    {
        await brokenLinkService.SaveChangesAsync([model]);
        return NoContent();
    }

    [HttpPatch]
    [Route("{id}")]
    [Authorize(ModuleConstants.Security.Permissions.Update)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<ActionResult> PatchAssociation(string id, [FromBody] JsonPatchDocument<BrokenLink> patchDocument)
    {
        if (patchDocument == null)
        {
            return BadRequest();
        }

        var brokenLink = await brokenLinkService.GetByIdAsync(id);
        if (brokenLink == null)
        {
            return NotFound();
        }

        patchDocument.ApplyTo(brokenLink, ModelState);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await brokenLinkService.SaveChangesAsync([brokenLink]);

        return NoContent();
    }

    [HttpDelete]
    [Route("")]
    [Authorize(ModuleConstants.Security.Permissions.Delete)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Delete([FromQuery] string[] ids)
    {
        await brokenLinkService.DeleteAsync(ids);
        return NoContent();
    }
}
