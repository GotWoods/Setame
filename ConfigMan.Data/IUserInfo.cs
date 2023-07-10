using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigMan.Data
{
    public interface IUserInfo
    {
        Guid GetCurrentUserId();
    }
}
