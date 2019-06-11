using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TraderApi.Models.Response
{
    public class AuthResponse : ResponseModel
    {
        public TokenViewModel Token { get; set; }

        public bool Blocked { get; set; }

        public DateTime BlockTime { get; set; }
    }
}
