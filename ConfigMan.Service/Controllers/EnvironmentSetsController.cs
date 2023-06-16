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
    private readonly IEnvironmentSetService _environmentSetService;

    public EnvironmentSetsController(IEnvironmentSetService environmentSetService)
    {
        _environmentSetService = environmentSetService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnvironmentSet>>> GetAll()
    {
        var deploymentEnvironments = await _environmentSetService.GetAllAsync();
        var sorted = deploymentEnvironments.ToList().OrderBy(x => x.Name);
        return Ok(sorted);
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<EnvironmentSet>> GetOne(string name)
    {
        var deploymentEnvironment = await _environmentSetService.GetOneAsync(name);
        return Ok(deploymentEnvironment);
    }

    [HttpPost]
    public async Task<ActionResult<EnvironmentSet>> Create(EnvironmentSet environmentSet)
    {
        await _environmentSetService.Create(environmentSet);
        return CreatedAtAction(nameof(Create), new { id = environmentSet.Name }, environmentSet);
    }

    [HttpPut("{name}")]
    public async Task<IActionResult> Update(string name, EnvironmentSet environmentSet)
    {
        await _environmentSetService.Update(environmentSet);
        return NoContent();
    }

    //DELETE: api/environments/guid
    [HttpDelete("{name}")]
    public async Task<IActionResult> Delete(string name)
    {
        await _environmentSetService.Delete(name);
        return NoContent();
    }
}