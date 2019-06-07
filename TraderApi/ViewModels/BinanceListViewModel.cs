using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TraderApi.ViewModels
{
    public class BinanceListViewModel
    {
        public long ID { get; set; }
        public DateTime Date { get; set; }
        public decimal Open { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal MinPrice { get; set; }
        public decimal Close { get; set; }
        public bool Deleted { get; set; }

        public BinanceListViewModel() { }

        public BinanceListViewModel(Domain.Quotation quotations)
        {
            ID = quotations.ID;
            Date = quotations.Date;
            Open = quotations.Open;
            MaxPrice = quotations.MaxPrice;
            MinPrice = quotations.MinPrice;
            Close = quotations.Close;
        }

        public static explicit operator BinanceListViewModel(Domain.Quotation quotations)
        {
            return new BinanceListViewModel(quotations);
        }
    }
}
