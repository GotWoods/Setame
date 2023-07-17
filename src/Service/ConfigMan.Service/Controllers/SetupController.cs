using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using ConfigMan.Service.Models;
using MediatR;
using ConfigMan.Data.Handlers;

namespace ConfigMan.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SetupController : ControllerBase
{
    private readonly IMediator _mediator;

    public SetupController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost()]
    public async Task<IActionResult> Setup(SetupRequest setupRequest)
    {
        await _mediator.Send(new InitializeApplication(setupRequest.AdminEmailAddress, setupRequest.NewPassword));
        return NoContent();
    }
}