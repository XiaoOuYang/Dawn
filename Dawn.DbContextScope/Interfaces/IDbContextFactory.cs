using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.DbContextScope.Interfaces
{
    public interface IDbContextFactory
    {
        TDbContext CreateDbContext<TDbContext>() where TDbContext : DbContext;
    }
}
