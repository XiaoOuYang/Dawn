using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.DbContextScope.Interfaces
{
    public interface IDbContextReadOnlyScope : IDisposable
    {
        /// <summary>
        /// The DbContext instances that this DbContextScope manages.
        /// </summary>
        IDbContextCollection DbContexts { get; }
    }
}
