using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TraderApi.Models.Response;

namespace TraderApi.ViewModels.Response
{
    public class KlineResponse : ResponseModel
    {
        public string[] Klines { get; set; } 
    }
}
