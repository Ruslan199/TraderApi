using Binance.Net.Objects;
using Domain.Enum;
using System;

namespace Domain
{
    public class QuotationOne: PersistentObject,IDeletableObject
    {
        public virtual DateTime Time { get; set; }
        public virtual string Pair { get; set; }
        public virtual decimal VolumeBase { get; set; }
        public virtual decimal VolumeQuote { get; set; }
        public virtual decimal Amplitude { get; set; }
        public virtual decimal DayPercentChange { get; set; }
        public virtual decimal DayVolumeChange { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
