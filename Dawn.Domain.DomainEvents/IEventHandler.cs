using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.Domain.DomainEvents
{
    public interface IEventHandler<TEvent>
        where TEvent : IEvent
    {
        Task Handle(TEvent @event);
    }
}
