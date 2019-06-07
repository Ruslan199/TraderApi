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

namespace TraderApi.BinanceTradeManager
{
    internal class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timerOne;
        private Timer _timerTwo;
        private Timer _timerThree;
        private Timer _timerFour;
        private Timer _timerFive;

       
        static object locker = new object();
        public IServiceProvider Services { get; }

        private static Example[] ex = new Example[7]
        {
            new Example { PairName = "GVTBTC", Pairs = Pairs.GVTBTC },
            new Example { PairName = "IOTXBTC", Pairs = Pairs.IOTXBTC },
            new Example { PairName = "STRATBTC", Pairs = Pairs.STRATBTC },
            new Example { PairName = "XRPBTC", Pairs = Pairs.XRPBTC },
            new Example { PairName = "WAVESBTC", Pairs = Pairs.WAVESBTC },
            new Example { PairName = "CMTBTC", Pairs = Pairs.CMTBTC },
            new Example { PairName = "BTCUSDT", Pairs = Pairs.BTCUSDT }
        };

        public TimedHostedService(ILogger<TimedHostedService> logger, IServiceProvider services)
        {
            Services = services;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var second = DateTime.Now.Second;
            var fiveMinute = DateTime.Now.Minute;
            var fifteenMinute = DateTime.Now.Minute;
            var oneHour = DateTime.Now.Hour;
            var fourHour = DateTime.Now.Hour;
            var oneDay = DateTime.Now.Hour;

            _logger.LogInformation("Timed Background Service is starting.");

          
                
                _timerOne = new Timer(DoWork, KlineInterval.FiveMinutes, TimeSpan.Zero, TimeSpan.FromMinutes(5));
           
                _timerTwo = new Timer(DoWork, KlineInterval.FiveteenMinutes, TimeSpan.Zero, TimeSpan.FromMinutes(15));
                _timerThree = new Timer(DoWork, KlineInterval.OneHour, TimeSpan.Zero, TimeSpan.FromHours(1));
                _timerFour = new Timer(DoWork, KlineInterval.FourHour, TimeSpan.Zero, TimeSpan.FromHours(4));
                _timerFive = new Timer(DoWork, KlineInterval.OneDay, TimeSpan.Zero, TimeSpan.FromDays(1));
           

            return Task.CompletedTask;

        }

        private void DoWork(object source)
        {
            _logger.LogInformation("Timed Background Service is working.");

            var gg = new List<Quotation>();
            using (var scope = Services.CreateScope())
            {
                var QuotationsFive =
                  scope.ServiceProvider
                  .GetRequiredService<IQuotationFiveService>();

                lock (locker)
                {
                    var timer = (KlineInterval)source;
                    var clientt = new BinanceClient();
                    clientt.SetApiCredentials("7ICQp7LXtOdaFOTYyQV4GqjA2nwOzkwgOW3KgHCQTj5fyZHXNHD4XmRVW3BukcXZ", "MFr1NGf1rLwSaNZ5pOfndWG4GX4vIRVUBpiCXqckKhj44rBAjracQb44M1n0Ra7g");
                    

                    foreach (var pairs in ex)
                    {
                        try
                        {
                            var requestKliness = clientt.GetKlines(pairs.PairName, timer, null, null, 2);
                            var example = requestKliness.Data[0];

                            var records = new Quotation
                            {
                                Date = example.OpenTime.ToLocalTime(),
                                Open = example.Open,
                                MaxPrice = example.High,
                                MinPrice = example.Low,
                                Close = example.Close,
                                Pair = pairs.Pairs,
                                Interval = timer
                            };

                            gg.Add(records);
                        }
                        catch
                        {
                            return;
                        }
                    }

                    QuotationsFive.CreateArray(gg);
                }
            }
            gg.Clear();
            //Dispose();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timerOne?.Change(Timeout.Infinite, 0);
            _timerTwo?.Change(Timeout.Infinite, 0);
            _timerThree?.Change(Timeout.Infinite, 0);
            _timerFour?.Change(Timeout.Infinite, 0);
            _timerFive?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timerOne?.Dispose();
            _timerTwo?.Dispose();
            _timerThree?.Dispose();
            _timerFour?.Dispose();
            _timerFive?.Dispose();
        }
    }
}
