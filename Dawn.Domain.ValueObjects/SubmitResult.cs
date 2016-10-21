using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.Domain.ValueObjects
{
    public class SubmitResult
    {
        public bool IsSucceed { get; set; } = true;

        public string Message { get; set; }
    }
}
