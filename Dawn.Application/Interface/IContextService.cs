using Dawn.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.Application.Interface
{
   public interface IContextService
    {
        /// <summary>
        /// 当前登录账号信息
        /// </summary>
        AccountIdentity CurrentAccount { get; }

        /// <summary>
        /// 清空当前登录人权限资料
        /// </summary>
        void ClearCurrentUser();
    }
}
