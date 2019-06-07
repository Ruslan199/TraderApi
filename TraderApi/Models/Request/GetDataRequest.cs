﻿using Binance.Net.Objects;
using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TraderApi.Models.Request
{
    public class GetDataRequest
    {
        public KlineInterval Interval { get; set; }
        public Pairs Pair { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
