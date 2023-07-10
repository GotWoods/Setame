using Marten.Events.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigMan.Data.Models.Projections
{
    public class UserSummary
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
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
