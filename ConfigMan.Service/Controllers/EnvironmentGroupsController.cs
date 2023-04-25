using ConfigMan.Data;
using ConfigMan.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConfigMan.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class EnvironmentGroupsController : ControllerBase
{
    private readonly IEnvironmentGroupService _environmentGroupService;

    public EnvironmentGroupsController(IEnvironmentGroupService environmentGroupService)
    {
        _environmentGroupService = environmentGroupService;
    }

    // GET: api/environments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnvironmentGroup>>> Get()
    {
        var environmentGroups = await _environmentGroupService.GetAllAsync();
        var sorted = environmentGroups.ToList().OrderBy(x => x.Name);
        return Ok(sorted);
    }

    // POST: api/environments
    [HttpPost]
    public async Task<ActionResult<DeploymentEnvironment>> Create(EnvironmentGroup environmentGroup)
    {
        await _environmentGroupService.CreateAsync(environmentGroup);
        return CreatedAtAction(nameof(Create), new { id = environmentGroup.Name }, environmentGroup);
    }

    // // GET: api/environments/guid
    // [HttpGet("{id}")]
    // public async Task<ActionResult<DeploymentEnvironment>> GetEnvironment(Guid id)
    // {
    //     var environment = await _environmentService.GetEnvironmentByIdAsync(id);
    //
    //     if (environment == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     return Ok(environment);
    // }

    // PUT: api/environments/guid
    // [HttpPut("{id}")]
    // public async Task<IActionResult> UpdateEnvironment(DeploymentEnvironment environment)
    // {
    //     await _environmentService.UpdateEnvironmentAsync(environment);
    //     return NoContent();
    // }

    // DELETE: api/environments/guid
    [HttpDelete("{name}")]
    public async Task<IActionResult> DeleteEnvironment(string name)
    {
        // await _environmentGroupService.(name);
         return NoContent();
    }
}