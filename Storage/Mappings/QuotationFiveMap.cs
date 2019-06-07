using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Text;
using Domain;
using Domain.Enum;
using Binance.Net.Objects;

namespace Storage.Mappings
{
    public class QuotationFiveMap : ClassMap<Quotation>
    {
        public QuotationFiveMap()
        {
            Table("quotations");
            Id(u => u.ID, "id");
            Map(u => u.Date, "request_time");
            Map(u => u.Open, "open_price");
            Map(u => u.MaxPrice, "max_price");
            Map(u => u.MinPrice, "min_price");
            Map(u => u.Close, "close_price");
            Map(u => u.Interval, "intervals").CustomType<KlineInterval>();
            Map(u => u.Pair, "pair").CustomType<Pairs>();
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
