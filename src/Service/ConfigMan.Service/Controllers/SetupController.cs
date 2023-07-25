﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using ConfigMan.Service.Models;
using MediatR;
using ConfigMan.Data.Handlers;
using System;

namespace ConfigMan.Service.Controllers;

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