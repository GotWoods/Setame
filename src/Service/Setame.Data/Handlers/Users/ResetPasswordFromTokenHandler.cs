using Marten;
using MediatR;
using Microsoft.Extensions.Logging;
using Setame.Data.Models;
using Setame.Data.Projections;

namespace Setame.Data.Handlers.Users;

public record ResetPasswordFromToken(string Token, string NewPassword) : IRequest<CommandResponse>;
public class ResetPasswordFromTokenHandler : IRequestHandler<ResetPasswordFromToken, CommandResponse>
{
    private readonly IQuerySession _querySession;
    private readonly IDocumentSessionHelper<User> _documentSession;
    private readonly IUserService _userService;
    private readonly ILogger<ResetPasswordFromToken> _logger;

    public ResetPasswordFromTokenHandler(IQuerySession querySession, IDocumentSessionHelper<User> documentSession, IUserService userService, ILogger<ResetPasswordFromToken> logger)
    {
        _querySession = querySession;
        _documentSession = documentSession;
        _userService = userService;
        _logger = logger;
    }

    public async Task<CommandResponse> Handle(ResetPasswordFromToken request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Resetting password from token {Token}", request.Token);
        var doc = _querySession.Query<PasswordResetSummary>().FirstOrDefault(x => x.Token == request.Token);

        if (doc == null)
        {
            _logger.LogWarning("Could not reset password as the token was not found");
            return CommandResponse.FromError(Errors.TokenNotFound);
        }

        if (doc.Expiration < DateTime.UtcNow)
        {
            _logger.LogWarning("Could not reset password as the token expired at {Expiry}", doc.Expiration);
            return CommandResponse.FromError(Errors.TokenExpired);
        }

        var salt = _userService.GenerateSalt();
        var hash = _userService.HashPassword(request.NewPassword, salt);
        await _documentSession.AppendToStreamAnonymously(doc.UserId, new PasswordReset(hash, salt));
        await _documentSession.SaveChangesAsync();
        
        _logger.LogDebug("Password reset");
        return CommandResponse.FromSuccess(-1);
    }
}