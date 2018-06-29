using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.Application.Interface
{
    public interface IAuthenticationApp
    {
        void SignIn(int accountId, string userName, bool createPersistentCookie);

        void SignOut();

    }
}

