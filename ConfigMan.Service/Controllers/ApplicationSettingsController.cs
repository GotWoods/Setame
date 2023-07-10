using System.Security.Claims;
using ConfigMan.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConfigMan.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class ApplicationSettingsController : ControllerBase
{
    private readonly IApplicationService _applicationService;
    private readonly ILogger<ApplicationSettingsController> _logger;

    public ApplicationSettingsController(IApplicationService applicationService, ILogger<ApplicationSettingsController> logger)
    {
        _applicationService = applicationService;
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

        return Ok();
    }

    [HttpPost("{applicationId}/{environment}")]
    public async Task<ActionResult> CreateNew(Guid applicationId, string environment, [FromBody] string variable, CancellationToken ct)
    {
        if (environment == "default")
            await _applicationService.Handle(new CreateDefaultApplicationVariable(applicationId, variable, ClaimsHelper.GetCurrentUserId(User)));

        else
            await _applicationService.Handle(new CreateApplicationVariable(applicationId, environment, variable, ClaimsHelper.GetCurrentUserId(User)));
        return CreatedAtAction(nameof(CreateNew), null);
    }

    [HttpPut("{applicationId}/{environment}/{variable}")]
    public async Task<ActionResult> Update(Guid applicationId, string environment, string variable, [FromBody] string value, CancellationToken ct)
    {
        if (applicationId == Guid.Empty)
            throw new ArgumentNullException("applicationId");
        if (string.IsNullOrEmpty(environment))
            throw new ArgumentNullException("environment");
        if (string.IsNullOrEmpty(variable))
            throw new ArgumentNullException("variable");
        if (string.IsNullOrEmpty(value))
            throw new ArgumentNullException("value");


        if (environment == "default")
            await _applicationService.Handle(new UpdateDefaultApplicationVariable(applicationId, variable, value, ClaimsHelper.GetCurrentUserId(User)));
        else
            await _applicationService.Handle(new UpdateApplicationVariable(applicationId, environment, variable, value, ClaimsHelper.GetCurrentUserId(User)));
        return Ok();
    }

    [HttpPost("{applicationId}/{variable}/rename")]
    public async Task<IActionResult> RenameVariable(Guid applicationId, string variable, [FromBody] string newName)
    {
        await _applicationService.Handle(new RenameApplicationVariable(applicationId, variable, newName, ClaimsHelper.GetCurrentUserId(User)));
        return Ok();
    }
}