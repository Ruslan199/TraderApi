using Domain;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storage.Mappings
{
    public class QuotationOneMap: ClassMap<QuotationOne>
    {
        public QuotationOneMap()
        {
            Table("quotations_minute");
            Id(u => u.ID, "id");
            Map(u => u.Time, "time");   
            Map(u => u.Pair, "pair");
            Map(u => u.VolumeBase, "volume_base");
            Map(u => u.VolumeQuote, "volume_quote");
            Map(u => u.Amplitude, "amplitude");
            Map(u => u.DayPercentChange, "day_percent");
            Map(u => u.DayVolumeChange, "day_volume");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
