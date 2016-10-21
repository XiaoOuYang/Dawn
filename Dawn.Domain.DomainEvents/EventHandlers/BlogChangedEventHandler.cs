using Dawn.Domain.DomainEvents.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.Domain.DomainEvents.EventHandlers
{
    public class BlogChangedEventHandler :
        IEventHandler<BlogChangedEvent>
    {
        public async Task Handle(BlogChangedEvent @event)
        {
            ///to do...
        }
    }
}
