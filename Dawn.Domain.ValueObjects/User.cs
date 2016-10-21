using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dawn.Domain.ValueObjects
{
    public class User
    {
        public string DisplayName { get; set; }

        public string Alias { get; set; }

        public DateTime? RegisterTime { get; set; }

        [JsonProperty("SpaceUserID")]
        public int Id { get; set; }
    }
}
