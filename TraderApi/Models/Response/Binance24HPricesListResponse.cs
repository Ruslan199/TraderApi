using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Binance.Net.Objects;

namespace TraderApi.Models.Response
{
    public class Binance24HPricesListResponse : ResponseModel
    {
        public decimal Volume { get; set; }
        public decimal VolumeBTC { get; set; }
        public decimal Percent { get; set; }
        public decimal PriceChangePercent { get; set; }
        public decimal QuoteVolume { get; set; }
    }
}
