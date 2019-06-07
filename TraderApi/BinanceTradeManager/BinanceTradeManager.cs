using Binance.Net;
using Binance.Net.Objects;
using BusinessLogic.Interfaces;
using Domain;
using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using Utils;

namespace TraderApi.BinanceTradeManager
{
    public class BinanceTradeManager
    {

        private IQuotationFiveService QuotationsFive { get; set; }
        //private IQuotationOneService QuotationsOne { get; set; }
        static object locker = new object();


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

        public BinanceTradeManager()
        { 
            var timers = new List<BinanceTimer>
            {
                new BinanceTimer(KlineInterval.FourHour),
                new BinanceTimer(KlineInterval.FiveMinutes),
                new BinanceTimer(KlineInterval.OneMinute),
                new BinanceTimer(KlineInterval.FiveteenMinutes),
                new BinanceTimer(KlineInterval.OneHour),
                new BinanceTimer(KlineInterval.OneDay)
            };
            var l = DateTime.Now;
            timers.ForEach(x =>
            {
                /*
                if (x.BinanceInterval == KlineInterval.OneMinute)
                {
                    x.Elapsed += new ElapsedEventHandler(LoadDataOneMinute);
                    x.Interval = 60000;
                    x.Start();
                }
                */
                if (x.BinanceInterval == KlineInterval.FiveMinutes)
                {
                    if (l.Minute == 0 || l.Minute == 5 || l.Minute == 10 || l.Minute == 15 || l.Minute == 20 || l.Minute == 25
                    || l.Minute == 30 || l.Minute == 35 || l.Minute == 40 || l.Minute == 45 || l.Minute == 50 || l.Minute == 55)
                    {
                        x.Elapsed += new ElapsedEventHandler(LoadData);
                        x.Interval = 60000 * 5;
                        x.Start();
                    }
                    else
                    {
                        return;
                    }
                }

                else if (x.BinanceInterval == KlineInterval.FiveteenMinutes)
                {
                    x.Elapsed += new ElapsedEventHandler(LoadData);
                    x.Interval = 60000 * 15;
                    x.Start();
                }

                else if (x.BinanceInterval == KlineInterval.OneHour)
                {
                    x.Elapsed += new ElapsedEventHandler(LoadData);
                    x.Interval = 60000 * 60;
                    x.Start();
                }

                else if (x.BinanceInterval == KlineInterval.FourHour)
                {
                    x.Elapsed += new ElapsedEventHandler(LoadData);
                    x.Interval = 60000 * 240;
                    x.Start();
                }
                else if (x.BinanceInterval == KlineInterval.OneDay)
                {
                    x.Elapsed += new ElapsedEventHandler(LoadData);
                    x.Interval = 60000 * 1440;
                    x.Start();

                }
                
            });
        }
        public void SetServices(IQuotationFiveService quotationService)//,IQuotationOneService quotationOne)
        {
            if (QuotationsFive == null)
                QuotationsFive = quotationService;

          /*  if (QuotationsOne == null)
                QuotationsOne = quotationOne;
                */
        }

        public void LoadData(object source, ElapsedEventArgs e)
        {
            lock(locker)
            {
                var timer = source as BinanceTimer;
                var clientt = new BinanceClient();
                clientt.SetApiCredentials("7ICQp7LXtOdaFOTYyQV4GqjA2nwOzkwgOW3KgHCQTj5fyZHXNHD4XmRVW3BukcXZ", "MFr1NGf1rLwSaNZ5pOfndWG4GX4vIRVUBpiCXqckKhj44rBAjracQb44M1n0Ra7g");
                var gg = new List<Quotation>();

                foreach (var pairs in ex)
                {
                    try
                    {
                        var requestKliness = clientt.GetKlines(pairs.PairName, timer.BinanceInterval, null, null, 2);
                        var example = requestKliness.Data[0];

                        var records = new Quotation
                        {
                            Date = example.OpenTime.ToLocalTime(),
                            Open = example.Open,
                            MaxPrice = example.High,
                            MinPrice = example.Low,
                            Close = example.Close,
                            Pair = pairs.Pairs,
                            Interval = timer.BinanceInterval
                        };

                        gg.Add(records);
                    }
                    catch
                    {
                        return;
                    }
                }
                QuotationsFive.CreateArray(gg);
               
                gg.Clear();
            }
        }
        /*
        public void LoadDataOneMinute(object source, ElapsedEventArgs e)
        {
            lock (locker)
            {
                var timer = source as BinanceTimer;
                var client = new BinanceClient();
                client.SetApiCredentials("7ICQp7LXtOdaFOTYyQV4GqjA2nwOzkwgOW3KgHCQTj5fyZHXNHD4XmRVW3BukcXZ", "MFr1NGf1rLwSaNZ5pOfndWG4GX4vIRVUBpiCXqckKhj44rBAjracQb44M1n0Ra7g");

                foreach (var pairs in ex)
                {
                    try
                    {
                        var requestHours = client.Get24HPrice(pairs.PairName);
                        var requestKlines = client.GetKlines(pairs.PairName, timer.BinanceInterval, null, null, 1);

                        decimal volume = requestKlines.Data[0].Volume;
                        decimal volumeBTC = requestKlines.Data[0].QuoteAssetVolume;

                        decimal percent = RequestHelper.CalculatePercent(requestKlines.Data);

                        decimal priceChangePercent = requestHours.Data.PriceChangePercent;
                        decimal quoteVolume = requestHours.Data.QuoteVolume;

                        var example = requestKlines.Data[0];

                        var records = new QuotationOne
                        {
                            Time = example.OpenTime.ToLocalTime(),
                            Pair = pairs.PairName,
                            VolumeBase = volume,
                            VolumeQuote = volumeBTC,
                            Amplitude = percent,
                            DayPercentChange = priceChangePercent,
                            DayVolumeChange = quoteVolume
                        };

                        QuotationsOne.Create(records);
                    }
                    catch
                    {
                        return;
                    }
                }
            }
        }
    */
    }
    public class Example
    {
        public string PairName { get; set; }
        public Pairs Pairs { get; set; }
    }
}


