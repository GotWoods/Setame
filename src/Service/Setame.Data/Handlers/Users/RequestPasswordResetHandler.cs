using Marten;
using MediatR;
using Microsoft.Extensions.Logging;
using Setame.Data.Models;
using Setame.Data.Projections;

namespace Setame.Data.Handlers.Users;

public record RequestPasswordReset(string EmailAddress) : IRequest<CommandResponse>;

public class RequestPasswordResetHandler : IRequestHandler<RequestPasswordReset, CommandResponse>
{
    private readonly IDocumentSessionHelper<User> _documentSession;
    private readonly ICallbackUrlProvider _callbackUrlProvider;
    private readonly IEmailService _emailService;
    private readonly ILogger<RequestPasswordResetHandler> _logger;
    private readonly IQuerySession _querySession;

    public RequestPasswordResetHandler(IQuerySession querySession, IEmailService emailService, IDocumentSessionHelper<User> documentSession, ICallbackUrlProvider callbackUrlProvider, ILogger<RequestPasswordResetHandler> logger)
    {
        _querySession = querySession;
        _emailService = emailService;
        _logger = logger;
        _documentSession = documentSession;
        _callbackUrlProvider = callbackUrlProvider;
    }

    public async Task<CommandResponse> Handle(RequestPasswordReset request, CancellationToken cancellationToken)
    {
        var user = await _querySession.Query<UserSummary>().FirstOrDefaultAsync(x => x.Username == request.EmailAddress, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("Password reset requested for non-existent email address of {Email}", request.EmailAddress);
            return CommandResponse.FromSuccess(-1); //return success so that an attacker can not find valid/invalid email addresses
        }

        var token = Guid.NewGuid().ToString();
        _logger.LogDebug("Reset token {Token} created for user {Email}", token, user.Id);
        await _documentSession.AppendToStreamAnonymously(user.Id, new PasswordResetRequested(token, DateTime.UtcNow.AddHours(1), user.Id));

        var email = new MailRequest
        {
            ToEmail = user.Username,
            Subject = "Setame Password Reset",
            Body = $"Please click the following link to reset your password: {_callbackUrlProvider.GetCallbackUrl()}/resetPassword?token={token}"
        };

        await _emailService.SendEmailAsync(email);
        await _documentSession.SaveChangesAsync();
        _logger.LogDebug("Reset email sent");
        return CommandResponse.FromSuccess(-1);
    }
}