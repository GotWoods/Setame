using Marten.Events.Projections;
using Setame.Data.Models;

namespace Setame.Data.Projections
{
    public class UserSummary
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
    }

    public class UsersProjection : MultiStreamProjection<UserSummary, Guid>
    {
        public UsersProjection()
        {
            // This is just specifying the aggregate document id
            // per event type. This assumes that each event
            // applies to only one aggregated view document
            Identity<UserCreated>(x => x.Id);
        }

        public void Apply(UserCreated e, UserSummary view)
        {
            view.Id = e.Id;
            view.Username = e.Username;
            view.PasswordHash = e.PasswordHash;
            view.Salt = e.Salt;
        }
    }
}
