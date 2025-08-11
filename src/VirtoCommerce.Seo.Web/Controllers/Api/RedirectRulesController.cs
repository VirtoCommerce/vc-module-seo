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
[Route("api/seo/redirect-rules")]
public class RedirectRulesController(
    IRedirectRuleSearchService redirectRuleSearchService,
    IRedirectRuleService redirectRuleService)
    : Controller
{
    [HttpGet]
    [Route("{id}")]
    [Authorize(ModuleConstants.Security.Permissions.Read)]
    public async Task<ActionResult<RedirectRule>> GetById(string id)
    {
        var contract = await redirectRuleService.GetNoCloneAsync(id);
        return Ok(contract);
    }

    [HttpPost]
    [Route("search")]
    [Authorize(ModuleConstants.Security.Permissions.Read)]
    public async Task<ActionResult<RedirectRuleSearchResult>> Search([FromBody] RedirectRuleSearchCriteria criteria)
    {
        var result = await redirectRuleSearchService.SearchNoCloneAsync(criteria);
        return Ok(result);
    }

    [HttpPost]
    [Route("")]
    [Authorize(ModuleConstants.Security.Permissions.Create)]
    public async Task<ActionResult<RedirectRule>> Create([FromBody] RedirectRule model)
    {
        await redirectRuleService.SaveChangesAsync([model]);
        return Ok(model);
    }

    [HttpPut]
    [Route("")]
    [Authorize(ModuleConstants.Security.Permissions.Update)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<ActionResult<RedirectRule>> Update([FromBody] RedirectRule model)
    {
        await redirectRuleService.SaveChangesAsync([model]);
        return NoContent();
    }

    [HttpPatch]
    [Route("{id}")]
    [Authorize(ModuleConstants.Security.Permissions.Update)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<ActionResult> PatchAssociation(string id, [FromBody] JsonPatchDocument<RedirectRule> patchDocument)
    {
        if (patchDocument == null)
        {
            return BadRequest();
        }

        var model = await redirectRuleService.GetByIdAsync(id);
        if (model == null)
        {
            return NotFound();
        }

        patchDocument.ApplyTo(model, ModelState);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await redirectRuleService.SaveChangesAsync([model]);

        return NoContent();
    }

    [HttpDelete]
    [Route("")]
    [Authorize(ModuleConstants.Security.Permissions.Delete)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Delete([FromQuery] string[] ids)
    {
        await redirectRuleService.DeleteAsync(ids);
        return NoContent();
    }
}
