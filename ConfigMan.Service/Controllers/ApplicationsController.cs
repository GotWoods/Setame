using ConfigMan.Data;
using ConfigMan.Data.Models;
using JasperFx.Core;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConfigMan.Service.Controllers.MyApplication.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _applicationService;

    public ApplicationsController(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Application>>> GetApplications(CancellationToken ct)
    {
        var items = await _applicationService.GetAll();
        var sorted = items.OrderBy(x => x.Name);
        return Ok(sorted);
    }

    [HttpGet("{applicationId}")]
    public async Task<ActionResult<Application>> GetApplication(Guid applicationId)
    {
        return Ok(await _applicationService.GetOne(applicationId));
    }

    [HttpPost]
    public async Task<IActionResult> CreateApplication(Application application, CancellationToken ct)
    {
        var id = CombGuidIdGeneration.NewGuid();
        await _applicationService.Handle(new CreateApplication(id, application.Name, application.Token, application.EnvironmentSetId, ClaimsHelper.GetCurrentUserId(User)));
        return NoContent();
    }

    [HttpDelete("{applicationId}")]
    public async Task<IActionResult> DeleteApplication(Guid applicationId)
    {
        await _applicationService.Handle(new DeleteApplication(applicationId, ClaimsHelper.GetCurrentUserId(User)));
        return NoContent();
    }
}