using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TraderApi.Models.Request
{
    public class UserRegistrationRequest
    {
        public string UserName { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
    }
}
