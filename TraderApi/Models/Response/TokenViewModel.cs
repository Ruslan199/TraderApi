using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TraderApi.Models.Response
{
    public class TokenViewModel
    {
        public string TokenStr { get; set; }

        public DateTime ExpireDate { get; set; }
    }
}
