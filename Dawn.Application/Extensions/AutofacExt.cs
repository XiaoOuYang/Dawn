using Autofac;
using System;
using System.Linq;

namespace Dawn.Application.Extensions
{
    /// <summary>
    ///  Autofac注册
    /// </summary>
    public class AutofacExt
    {
        /// <summary>
        /// Ioc初始化容器
        /// </summary>
        public static void Initialize()
        {
            var builder = new ContainerBuilder();

           // var assembly = AppDomain.CurrentDomain.GetAssemblies().Where(q => q.FullName.Contains("")).ToArray();

           // builder.RegisterControllers(assembly);//注册mvc容器的实现 

            ////注册app层
            //builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(AuthenticationMvcApp)))
            //    .Where(q => q.Namespace.Equals("Examda.Boss.App"))
            //    .AsImplementedInterfaces();

            ////builder.RegisterType(typeof(AuthenticationMvcApp));

           
            //builder.RegisterType(typeof(IOHelper)).As(typeof(IOHelper));
            //builder.RegisterInstance<ILoggerFactory>(new Log4NetLoggerFactory());

            //builder.RegisterType<DefaultTypeNameProvider>().As<ITypeNameProvider>().SingleInstance();
            //builder.RegisterType<DefaultMessageHandlerProvider>().As<IMessageHandlerProvider>().SingleInstance();

            //builder.RegisterType<TestCacheService>().As<ICacheService>().AsImplementedInterfaces();

            //IocContainer.Builder(builder);

            //IocContainer.Resolve<ITypeNameProvider>().Initialize(Assembly.Load("Examda.Boss.Domain"));
            //IocContainer.Resolve<IMessageHandlerProvider>().Initialize(Assembly.Load("Examda.Boss.Domain.Services"),
            //    Assembly.Load("Examda.Boss.App"));

            //DependencyResolver.SetResolver(new AutofacDependencyResolver(IocContainer.Instance));

            //InitDbConexnt.Initialize();


        }
    }
}