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

        public AuthenticationController(IUserService userService, AuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            
            var user = _userService.GetUserByUsernameAsync(request.Username);

            if (user == null || !_userService.VerifyPassword(user, request.Password))
            {
                if (request.Username == "admin" && request.Password == "admin") //this is an initial login to the system (as the previous check found the user was null)
                {
                    var newAdminUser = new User();
                    newAdminUser.Username = request.Username;
                    newAdminUser.Id = Guid.NewGuid();
                    await _userService.CreateUserAsync(newAdminUser, request.Password);
                    var newUserToken  = _authService.GenerateJwtToken(newAdminUser.Id.ToString(), "Administrator");
                    return Ok(new { newUserToken });
                }

                return Unauthorized();
            }

            var token = _authService.GenerateJwtToken(user.Id.ToString(), "Administrator");
            return Ok(new { token });
        }

        [HttpPost("AppLogin")]
        public async Task<IActionResult> AppLogin([FromBody] AppLoginRequest request)
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