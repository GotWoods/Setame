using Marten.Events.Projections;
using Marten.Schema;
using Setame.Data.Models;

namespace Setame.Data.Projections;

public class PasswordResetSummary
{
    [Identity] public string Token { get; set; }

    public DateTime Expiration { get; set; }
    public Guid UserId { get; set; }
}

public class PendingPasswordResetProjection : MultiStreamProjection<PasswordResetSummary, string>
{
    public PendingPasswordResetProjection()
    {
        DeleteEvent<PasswordReset>();
        Identity<PasswordResetRequested>(x => x.Token);
    }

    public void Apply(PasswordResetRequested e, PasswordResetSummary view)
    {
        view.Token = e.Token;
        view.Expiration = e.Expiration;
        view.UserId = e.UserId;
    }
}