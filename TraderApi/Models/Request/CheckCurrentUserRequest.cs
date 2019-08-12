using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TraderApi.Models.Request
{
    public class CheckCurrentUserRequest
    {
        [Required]
        public string SocketId { get; set; }
    }
}
