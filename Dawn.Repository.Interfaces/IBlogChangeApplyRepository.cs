using Dawn.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.Repository.Interfaces
{
    public interface IBlogChangeApplyRepository : IApplyRepository<BlogChangeApply>
    {
        IQueryable<BlogChangeApply> GetByTargetAliasWithWait(string targetBlogApp);
    }
}
