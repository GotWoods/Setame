using MediatR;
using Microsoft.Extensions.Logging;
using Setame.Data.Projections;

namespace Setame.Data.Handlers.Users;

public record UserLogin(string Username, string Password) : IRequest<CommandResponseData<UserSummary>>;

public class UserLoginHandler : IRequestHandler<UserLogin, CommandResponseData<UserSummary>>
{
    private readonly ILogger<UserLoginHandler> _logger;
    private readonly IUserService _userService;

    public UserLoginHandler(IUserService userService, ILogger<UserLoginHandler> logger)
    {
        _userService = userService;
        _logger = logger;
    }


    public Task<CommandResponseData<UserSummary>> Handle(UserLogin request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Login requested for {Username}", request.Username);
        var user = _userService.GetUserByUsernameAsync(request.Username);

        if (user == null)
        {
            _logger.LogDebug("User not found");
            return Task.FromResult(CommandResponseData<UserSummary>.FromError(Errors.AuthenticationFailed));
        }

        if (!_userService.VerifyPassword(user, request.Password))
        {
            _logger.LogDebug("Password verification failed");
            return Task.FromResult(CommandResponseData<UserSummary>.FromError(Errors.AuthenticationFailed));
        }

        return Task.FromResult(CommandResponseData<UserSummary>.FromSuccess(user, -1));
    }
}