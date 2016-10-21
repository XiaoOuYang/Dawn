using Dawn.DbContextScope.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.DbContextScope.Impl
{
    public class AmbientDbContextLocator : IAmbientDbContextLocator
    {
        public TDbContext Get<TDbContext>() where TDbContext : DbContext
        {
            var ambientDbContextScope = DbContextScope.GetAmbientScope();
            return ambientDbContextScope == null ? null : ambientDbContextScope.DbContexts.Get<TDbContext>();
        }
    }
}
