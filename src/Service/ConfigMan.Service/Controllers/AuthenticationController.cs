using ConfigMan.Data;
using ConfigMan.Data.Models;
using ConfigMan.Service.Models;
using JasperFx.Core;
using Marten;
using Microsoft.AspNetCore.Mvc;

namespace ConfigMan.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly AuthService _authService;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(IUserService userService, AuthService authService, IEmailService emailService, ILogger<AuthenticationController> logger)
        {
            _userService = userService;
            _authService = authService;
            _emailService = emailService;
            _logger = logger;
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

            // var mailRequest = new MailRequest();
            // mailRequest.ToEmail = "dave@solidhouse.com";
            // mailRequest.Subject = "Hello!";
            // mailRequest.Body = "Someone logged in";
            //
            // await _emailService.SendEmailAsync(mailRequest);
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