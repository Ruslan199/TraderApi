using Binance.Net.Objects;
using Domain.Enum;
using System;

namespace Domain
{
    public class Quotation : PersistentObject, IDeletableObject
    {
        public virtual DateTime Date { get; set; }
        public virtual decimal Open { get; set; }
        public virtual decimal MaxPrice { get; set; }
        public virtual decimal MinPrice { get; set; }
        public virtual decimal Close { get; set; }
        public virtual Pairs Pair { get; set; }
        public virtual KlineInterval Interval { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
