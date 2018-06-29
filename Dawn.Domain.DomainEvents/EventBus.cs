using Dawn.Infrastructure.Interfaces;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace Dawn.Domain.DomainEvents
{
    public class EventBus : IEventBus
    {
      

        public async Task Publish<TEvent>(TEvent @event)
            where TEvent : IEvent
        {
            var eventHandler = IocContainer.Instance.Resolve<IEventHandler<TEvent>>();
            await eventHandler.Handle(@event);
        }
    }
}
