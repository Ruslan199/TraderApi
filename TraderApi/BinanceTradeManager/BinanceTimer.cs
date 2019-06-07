using Binance.Net.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace TraderApi.BinanceTradeManager
{
    public class BinanceTimer : Timer
    {
        private KlineInterval _klineInterval { get; set; }

        public KlineInterval BinanceInterval
        {
            get => _klineInterval;
        }

        public BinanceTimer(KlineInterval interval)
        {
            _klineInterval = interval;
        }
    }
}
