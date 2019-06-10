using Binance.Net.Objects;
using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TraderApi.Models.Request
{
    public class DataOfRealTimeRequest
    {
        public DateTime Time { get; set; }
        public KlineInterval Interval { get; set; }
        public Pairs Pair { get; set; }
        public decimal Inaccuracy { get; set; }
        public int Value { get; set; }
        public string Login { get; set; }
    }
}
