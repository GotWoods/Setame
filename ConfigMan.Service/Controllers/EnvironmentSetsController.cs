using ConfigMan.Data;
using ConfigMan.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConfigMan.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class EnvironmentSetsController : ControllerBase
{
    private readonly IEnvironmentService _environmentService;

    public EnvironmentSetsController(IEnvironmentService environmentService)
    {
        _environmentService = environmentService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnvironmentSet>>> GetEnvironments()
    {
        var deploymentEnvironments = await _environmentService.GetAllAsync();
        var sorted = deploymentEnvironments.ToList().OrderBy(x => x.Name);
        return Ok(sorted);
    }

    [HttpPost]
    public async Task<ActionResult<EnvironmentSet>> CreateEnvironment(EnvironmentSet environment)
    {
        await _environmentService.Create(environment);
        return CreatedAtAction(nameof(CreateEnvironment), new { id = environment.Name }, environment);
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

    [HttpPut("{name}")]
    public async Task<IActionResult> UpdateEnvironment(EnvironmentSet environment)
    {
        await _environmentService.UpdateAsync(environment);
        return NoContent();
    }

    // DELETE: api/environments/guid
    // [HttpDelete("{name}")]
    // public async Task<IActionResult> DeleteEnvironment(string name)
    // {
    //     await _environmentService.DeleteEnvironmentAsync(name);
    //     return NoContent();
    // }
}