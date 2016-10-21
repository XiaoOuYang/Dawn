using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.DbContextScope.Interfaces
{
    public interface IDbContextCollection : IDisposable
    {
        /// <summary>
        /// Get or create a DbContext instance of the specified type. 
        /// </summary>
		TDbContext Get<TDbContext>() where TDbContext : DbContext;
    }
}
