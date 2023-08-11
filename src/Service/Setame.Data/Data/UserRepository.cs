using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Marten;
using Org.BouncyCastle.Asn1.Ocsp;
using Setame.Data.Projections;

namespace Setame.Data.Data
{
    public class UserRepository
    {
        private readonly IQuerySession _querySession;

        public UserRepository(IQuerySession querySession)
        {
            _querySession = querySession;
        }

        public async Task<UserSummary?> GetActiveUser(string email)
        {
            return await _querySession.Query<UserSummary>().FirstOrDefaultAsync(x => x.Username == email);
        }

        public PasswordResetSummary? GetPasswordResetSummary(string token)
        {
            return _querySession.Query<PasswordResetSummary>().FirstOrDefault(x => x.Token == token);
        }
    }
}
