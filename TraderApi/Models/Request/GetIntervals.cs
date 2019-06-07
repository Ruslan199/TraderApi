using Binance.Net.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TraderApi.Models.Request
{
    public class GetIntervals
    {
        public KlineInterval Interval { get; set; }
        public string Name { get; set; }
    }
}
