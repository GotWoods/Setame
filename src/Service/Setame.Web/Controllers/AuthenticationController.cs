using MediatR;
using Microsoft.AspNetCore.Mvc;
using Setame.Data;
using Setame.Data.Handlers.Users;
using Setame.Web.Models;

namespace Setame.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly AuthService _authService;
        private readonly IMediator _mediator;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(IUserService userService, AuthService authService, IMediator mediator, ILogger<AuthenticationController> logger)
        {
            _userService = userService;
            _authService = authService;
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] string username)
        {
            var results = await _mediator.Send(new RequestPasswordReset(username));
            return NoContent();
        }

        [HttpPut("{token}/ForgotPasswordReset")]
        public async Task<IActionResult> ForgotPasswordReset(string token, [FromBody] string newPassword)
        {
            var results = await _mediator.Send(new ResetPasswordFromToken(token, newPassword));
            if (!results.WasSuccessful)
                return BadRequest(results);
            return Ok();
        }

        [HttpPost("login")]
        public Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            _logger.LogDebug("Login requested for {Username}", request.Username);
            var user = _userService.GetUserByUsernameAsync(request.Username);

            if (user == null)
            {
                _logger.LogDebug("User not found");
                return Task.FromResult<IActionResult>(Unauthorized());
            }

            if (!_userService.VerifyPassword(user, request.Password))
            {
                _logger.LogDebug("Password verification failed");
                return Task.FromResult<IActionResult>(Unauthorized());
            }
            _logger.LogDebug("User credentials verified, generating token");
            var token = _authService.GenerateJwtToken(user.Id.ToString(), "Administrator");
            return Task.FromResult<IActionResult>(Ok(new { token }));
        }

        [HttpPost("AppLogin")]
        public IActionResult AppLogin([FromBody] AppLoginRequest request)
        {
            //TODO: hash this token with the secret key? Then the calling app needs the key too though
            //  var application = await _applicationService.GetApplicationByIdAsync(request.ApplicaitonName);
            //
            //  if (application == null || application.Token != request.Token)
            //  {
            //      return Unauthorized();
            //  }
            //
            //  var token = _authService.GenerateJwtToken(application.Name, "Application");
            // return Ok(new { token });
            return Ok();
        }
    }
}