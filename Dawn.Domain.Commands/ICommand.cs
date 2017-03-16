using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.Domain.Commands
{
    public interface ICommand
    {
        int Id { get; set; }
        DateTime Timestamp { get; set; }
    }
}
