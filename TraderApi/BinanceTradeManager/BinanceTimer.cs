using Binance.Net.Objects;
using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace TraderApi.BinanceTradeManager
{
    public class BinanceTimer : Timer
    {
        private KlineInterval _kline { get; set; }

        public KlineInterval BinanceInterval
        {
            get => _kline;
        }

        public BinanceTimer(KlineInterval kline)
        {
            _kline = kline;
        }
    }
}
