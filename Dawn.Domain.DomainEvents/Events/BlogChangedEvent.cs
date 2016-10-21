using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.Domain.DomainEvents.Events
{
    public class BlogChangedEvent : IEvent
    {
        public string UserAlias { get; set; }

        public string TargetUserAlias { get; set; }
    }
}
