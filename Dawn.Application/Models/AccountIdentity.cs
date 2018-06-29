using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.Application.Models
{
    [Serializable]
    public class AccountIdentity
    {
        public AccountIdentity(int accountId, string account)
        {
            this.AccountId = accountId;
            this.Account = account;
        }
        public int AccountId { get; set; }

        public string Account { get; set; }

    }
}
