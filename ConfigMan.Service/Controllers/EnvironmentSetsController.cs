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
        await _environmentSetService.CreateAsync(environmentSet);
        return NoContent();
    }

    [HttpPut("{name}")]
    public async Task<IActionResult> Update(string name, EnvironmentSet environmentSet)
    {
        await _environmentSetService.UpdateAsync(environmentSet);
        return NoContent();
    }

    [HttpDelete("{name}")]
    public async Task<IActionResult> Delete(string name)
    {
    
        await _environmentSetService.DeleteAsync(name);
        return NoContent();
    }


    [HttpPost("{name}/rename")]
    public async Task<IActionResult> RenameVariable(string name, [FromBody] string newName)
    {
        await _environmentSetService.RenameAsync(name,  newName);
        return NoContent();
    }
}