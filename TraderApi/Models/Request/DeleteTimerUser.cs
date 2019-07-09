using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TraderApi.Models.Request
{
    public class DeleteTimerUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}
