using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class User : PersistentObject, IDeletableObject
    {
        public virtual DateTime Date { get; set; }
        public virtual DateTime Time { get; set; }
        public virtual string Symbol { get; set; }
        public virtual decimal PurchaseAmount { get; set; }
        public virtual decimal PurchaseAmountBTC { get; set; }
        public virtual decimal Amplitude { get; set; }
        public virtual decimal AmplitudeSound { get; set; }
        public virtual decimal DayPercentChange { get; set; }
        public virtual decimal SumDayVolumeBTC { get; set; }
        public virtual bool Deleted { get; set; }
    }
}
