using Dawn.Domain.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.CommandHandlers
{
    public class BlogCommandHandler : ICommandHandler<BlogChangeApply>
    {
        public void Handle(BlogChangeApply command)
        {
            throw new NotImplementedException();
        }
    }
}
