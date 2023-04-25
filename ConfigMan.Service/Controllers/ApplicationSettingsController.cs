using ConfigMan.Data;
using ConfigMan.Data.Models;
using ConfigMan.Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ConfigMan.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class ApplicationSettingsController : ControllerBase
{
    private readonly IApplicationService _applicationService;
    private readonly IEnvironmentService _environmentService;
    private readonly ILogger<ApplicationSettingsController> _logger;

    public ApplicationSettingsController(IApplicationService applicationService, IEnvironmentService environmentService, ILogger<ApplicationSettingsController> logger)
    {
        _applicationService = applicationService;
        _environmentService = environmentService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "Application")]
    public async Task<ActionResult> Get() 
    {
        
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
        {
            _logger.LogWarning("Can not find application @{Claims}", User.Claims);
            return NotFound("Claim not found");
        }

        var env = await _environmentService.GetOneAsync("Dev");
        var application = await _applicationService.GetApplicationByIdAsync(claim.Value);
        return Ok(); // application.GetAppliedSettings(env));

    }

    // POST: api/environments
    [HttpPost]
    public async Task<ActionResult> CreateApplicationSetting(CreateApplicationSettingRequest request)
    {
        foreach (var environmentSetting in request.Settings)
        {
            var setting = new Setting();
            setting.Name = environmentSetting.Name;
            setting.Value = environmentSetting.Value;

            if (environmentSetting.Environment == "Default")
                await _applicationService.AddApplicationSetting(request.ApplicationId, setting);
            else
                await _applicationService.AddEnvironmentSetting(request.ApplicationId, environmentSetting.Environment, setting);
        }
        return CreatedAtAction(nameof(CreateApplicationSetting), null);
    }
}