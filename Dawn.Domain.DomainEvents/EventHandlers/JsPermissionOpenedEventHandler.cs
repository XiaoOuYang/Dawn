using Dawn.Domain.DomainEvents.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.Domain.DomainEvents.EventHandlers
{
    public class JsPermissionOpenedEventHandler :
         IEventHandler<JsPermissionOpenedEvent>
    {
        public async Task Handle(JsPermissionOpenedEvent @event)
        {
            //await BlogService.EnableJsPermission(@event.UserAlias);
            throw new Exception("''''");
        }
    }
}
