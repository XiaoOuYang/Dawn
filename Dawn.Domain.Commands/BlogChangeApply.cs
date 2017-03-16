using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.Domain.Commands
{

    public class BlogChangeApply : ICommand
    {
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }

        public string targetBlogApp { get; set; }
        public string reason { get; set; }
        public string userLoginName { get; set; }
        public string ip { get; set; }

        public BlogChangeApply(string targetBlogApp, string reason, string userLoginName,string ip)
        {
            Timestamp = DateTime.Now;
            this.targetBlogApp = targetBlogApp;
            this.reason = reason;
            this.userLoginName = userLoginName;
            this.ip = ip;
        }


    }
}
