using MediatR;
using Microsoft.AspNetCore.Mvc;
using Setame.Data.Handlers;
using Setame.Web.Models;

namespace Setame.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SetupController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SetupController> _logger;

    public SetupController(IMediator mediator, ILogger<SetupController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost()]
    public async Task<IActionResult> Setup(SetupRequest setupRequest)
    {
        _logger.LogDebug("Setup request received");
        await _mediator.Send(new InitializeApplication(setupRequest.AdminEmailAddress, setupRequest.NewPassword));
        return NoContent();
    }
}