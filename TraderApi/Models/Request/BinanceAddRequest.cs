using Binance.Net.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TraderApi.Models.Request
{
    public class BinanceAddRequest
    {
        public string Pair { get; set; }
        public KlineInterval Interval { get; set; }
    }
}
