using Dawn.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.Application.Interfaces
{
    public interface IBlogChangeApplyService
    {
        Status GetStatus(string userLoginName);

        SubmitResult Apply(string targetBlogApp, string reason, string userLoginName, string ip);
    }
}
