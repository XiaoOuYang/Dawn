using Dawn.Domain.DomainEvents.Events;
using Dawn.ServiceAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.Domain.DomainEvents.EventHandlers
{
    public class MessageSentEventHandler :
          IEventHandler<MessageSentEvent>
    {
        public async Task Handle(MessageSentEvent @event)
        {
            await MsgService.Send(@event.Title, @event.Content, @event.RecipientId);
        }
    }
}
