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
        private int i;

        private bool five;
        private bool fifteen;

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
            _logger.LogInformation("Timed Background Service is starting.");
            System.Timers.Timer myTimer = new System.Timers.Timer();
            myTimer.Elapsed += new System.Timers.ElapsedEventHandler(Work);
            myTimer.Interval = 1000; // 1000 ms is one second
            myTimer.Start();
            
            return Task.CompletedTask;

        }
        private void Work(object source, EventArgs s)
        {
            var FiveMinute = DateTime.Now.Minute % 5;//FiveMinute == 0 
            var FifteenMinute = DateTime.Now.Minute % 15;
            var Seconds = DateTime.Now.Second;
            if (FiveMinute == 0 && Seconds == 0)
            {
                Console.WriteLine(DateTime.Now);
                while (five != true)
                {
                    _timerOne = new Timer(DoWork, KlineInterval.FiveMinutes, TimeSpan.Zero, TimeSpan.FromMinutes(5));
                    five = true;
                    i++;
                }
                
            }
            if (DateTime.Now.Minute == 1 && Seconds == 30)//FifteenMinute == 0
            {
                while (fifteen != true)
                {
                    _timerTwo = new Timer(DoWorkRest, KlineInterval.FiveteenMinutes, TimeSpan.Zero, TimeSpan.FromMinutes(15));
                    fifteen = true;
                    i++;
                }

            }

        }

        private void DoWork(object source)
        {
            _logger.LogInformation("Timed Background Service is working.");
            var timer = (KlineInterval)source;
            var clientt = new BinanceClient();
            clientt.SetApiCredentials("7ICQp7LXtOdaFOTYyQV4GqjA2nwOzkwgOW3KgHCQTj5fyZHXNHD4XmRVW3BukcXZ", "MFr1NGf1rLwSaNZ5pOfndWG4GX4vIRVUBpiCXqckKhj44rBAjracQb44M1n0Ra7g");
            var gg = new List<Quotation>();
            var example = new BinanceKline();

            using (var scope = Services.CreateScope())
            {
                var QuotationsFive =
                  scope.ServiceProvider
                  .GetRequiredService<IQuotationFiveService>();

                lock (locker)
                {
                    foreach (var pairs in ex)
                    {
                        try
                        {
                            var requestKliness = clientt.GetKlines(pairs.PairName, timer, null, null, 2);

                            var checkOne = DateTime.Now.Minute - requestKliness.Data[0].OpenTime.Minute;
                            var checkTwo = DateTime.Now.Minute - requestKliness.Data[1].OpenTime.Minute;

                            if (checkOne == 10 || checkTwo == 5)
                            {
                                example = requestKliness.Data[1];
                            }
                            else
                            {
                                example = requestKliness.Data[0];
                            }
                                  
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
                    Console.WriteLine(DateTime.Now);
                    gg.Clear();
                }
            }
        }

        private void DoWorkRest(object source)
        {
            _logger.LogInformation("Timed Background Service is working.");
            var timer = (KlineInterval)source;
            var clientt = new BinanceClient();
            clientt.SetApiCredentials("7ICQp7LXtOdaFOTYyQV4GqjA2nwOzkwgOW3KgHCQTj5fyZHXNHD4XmRVW3BukcXZ", "MFr1NGf1rLwSaNZ5pOfndWG4GX4vIRVUBpiCXqckKhj44rBAjracQb44M1n0Ra7g");
            var gg = new List<Quotation>();
            var example = new BinanceKline();

            using (var scope = Services.CreateScope())
            {
                var QuotationsFive =
                  scope.ServiceProvider
                  .GetRequiredService<IQuotationFiveService>();

                lock (locker)
                {
                    foreach (var pairs in ex)
                    {
                        try
                        {
                            var requestKliness = clientt.GetKlines(pairs.PairName, timer, null, null, 2);
                            
                            example = requestKliness.Data[0];

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
                    Console.WriteLine(DateTime.Now);
                    gg.Clear();
                }
            }
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

