using AutoMapper;
using Dawn.DbContextScope.Impl;
using Dawn.DbContextScope.Interfaces;
using Dawn.Domain.DomainEvents;
using Dawn.Domain.DomainEvents.EventHandlers;
using Dawn.Domain.DomainEvents.Events;
using Dawn.Domain.DomainServices;
using Dawn.Domain.Entity;
using Dawn.Infrastructure.Interfaces;
using Dawn.Repository.EF;
using Dawn.Repository.Interfaces;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.Apply.BootStrapper
{
    public class Startup
    {

        public static void Configure()
        {
            UnityContainer register = IocContainer.Default;

             
            register.RegisterType<IAmbientDbContextLocator, AmbientDbContextLocator>(new PerResolveLifetimeManager());
            register.RegisterType<IDbContextScopeFactory, DbContextScopeFactory>(new PerResolveLifetimeManager());

            register.RegisterType<IJsPermissionApplyRepository, JsPermissionApplyRepository>();
            register.RegisterType<IBlogChangeApplyRepository, BlogChangeApplyRepository>();

            register.RegisterType<IEventBus, EventBus>();
            register.RegisterType<IEventHandler<MessageSentEvent>, MessageSentEventHandler>();
            register.RegisterType<IEventHandler<JsPermissionOpenedEvent>, JsPermissionOpenedEventHandler>();
            register.RegisterType<IEventHandler<BlogChangedEvent>, BlogChangedEventHandler>();

            register.RegisterType<IApplyAuthenticationService, ApplyAuthenticationService>();

            ConfigureMapper();
        }

        public static void ConfigureMapper()
        {
            Mapper.Initialize(cfg =>
            {
                //cfg.CreateMap<JsPermissionApply, JsPermissionApplyDTO>();
                //cfg.CreateMap<BlogChangeApply, BlogChangeApplyDTO>();
            });
        }

    }
}
