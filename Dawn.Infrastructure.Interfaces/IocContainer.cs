using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.Infrastructure.Interfaces
{
    public class IocContainer
    {
        public static UnityContainer Default = new UnityContainer();
    }
}
