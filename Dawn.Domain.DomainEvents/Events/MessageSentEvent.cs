using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.Domain.DomainEvents.Events
{
    public class MessageSentEvent : IEvent
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public int RecipientId { get; set; }
    }
}
