using ConfigMan.Data;
using ConfigMan.Data.Models;
using ConfigMan.Service.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConfigMan.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EnvironmentSettingsController : ControllerBase
{
    private readonly IEnvironmentService _environmentService;

    public EnvironmentSettingsController(IEnvironmentService environmentService)
    {
        _environmentService = environmentService;
    }

    // POST: api/environments
    [HttpPost]
    public async Task<ActionResult<DeploymentEnvironment>> CreateEnvironmentSetting(List<EnvironmentSetting> settings)
    {
        foreach (var environmentSetting in settings)
        {
            var setting = new Setting() { Name = environmentSetting.Name, Value = environmentSetting.Value };
            await _environmentService.AddSettingToEnvironment(environmentSetting.Environment, setting);
        }
        return CreatedAtAction(nameof(CreateEnvironmentSetting), null);
    }
}