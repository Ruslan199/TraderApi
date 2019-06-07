
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TraderApi.BinanceTradeManager;
using TraderApi.Models.Request;
using TraderApi.ViewModels.Response;

namespace TraderApi.Interface
{
    public interface ITimerService
    {
        List<BinancePair> timer { get; set; }
        List<DataOfRealTimeRequest> data { get; set; }
        void Count(object obj, System.Timers.ElapsedEventArgs e);
    }
}
