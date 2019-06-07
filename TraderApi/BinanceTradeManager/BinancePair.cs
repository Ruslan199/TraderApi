using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using TraderApi.Models.Request;

namespace TraderApi.BinanceTradeManager
{
    public class BinancePair : Timer
    {
        private Pairs _pair { get; set; }

        public Pairs BinancePairr
        {
            get => _pair;
        } 

        public BinancePair(Pairs pair)
        {
            _pair = pair;
        }
    }
}
