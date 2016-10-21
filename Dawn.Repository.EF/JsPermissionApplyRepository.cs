using Dawn.Domain.Entity;
using Dawn.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dawn.DbContextScope.Interfaces;

namespace Dawn.Repository.EF
{
    public class JsPermissionApplyRepository : ApplyRepository<ApplyDbContext, JsPermissionApply>, IJsPermissionApplyRepository
    {
        public JsPermissionApplyRepository(IAmbientDbContextLocator ambientDbContextLocator) : base(ambientDbContextLocator)
        {
        }
    }
}
