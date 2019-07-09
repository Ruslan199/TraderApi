using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TraderApi.Models.Request
{
    public class CurrentUser
    {
        public int id { get; set; }
        public string User { get; set; }
        public System.Timers.Timer timer { get; set; }
    }
}
