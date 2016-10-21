using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.Domain
{
    public interface IAggregateRoot
    {
        int Id { get; }
    }
}
