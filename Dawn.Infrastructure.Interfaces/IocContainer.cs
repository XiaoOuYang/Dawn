using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.Infrastructure.Interfaces
{
    public class IocContainer
    {

        private static IContainer _container;

        public static IContainer Instance
        {
            get
            {
                return _container;
            }
        }
        public static IContainer Builder(ContainerBuilder builder)
        {


            _container = builder.Build();
            return _container;
        }

        public static T Resolve<T>()
        {
            return Instance.Resolve<T>();
        }

        public static object Resolve(System.Type serviceType)
        {
            return Instance.Resolve(serviceType);
        }
    }
}
