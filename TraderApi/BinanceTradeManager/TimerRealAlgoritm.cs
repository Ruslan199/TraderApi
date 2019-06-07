using Binance.Net;
using Binance.Net.Objects;
using BusinessLogic.Interfaces;
using Domain;
using Domain.Enum;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace TraderApi.BinanceTradeManager
{
    internal class TimerRealAlgoritm
    {

        static object locker = new object();
        public IServiceProvider Services { get; }

        public TimerRealAlgoritm(IServiceProvider services)
        {
            Services = services;
            
        }

  
  /*      private string DoWork(decimal innarcy)
        {
            using (var scope = Services.CreateScope())
            {
                var QuotationsFive =
                  scope.ServiceProvider
                  .GetRequiredService<IQuotationFiveService>();

                var Data = QuotationsFive.GetAll().Where(x => x.Pair == request.Pair && x.Interval == request.Interval && x.Date < time.ToLocalTime()).OrderByDescending(x => x.ID).Take(7).ToList();
                var DataRed = QuotationsFive.GetAll().Where(x => x.Pair == request.Pair && x.Interval == request.Interval && x.Date < time).OrderByDescending(x => x.ID).Take(7).ToList();

            }
        }
        */
    }
}


