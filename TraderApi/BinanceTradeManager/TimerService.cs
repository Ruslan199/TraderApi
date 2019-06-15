using BusinessLogic.Interfaces;
using Domain.Enum;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TraderApi.Interface;
using TraderApi.Models.Request;
using TraderApi.ViewModels.Response;
using Microsoft.Extensions.DependencyInjection;
using System.Net.WebSockets;
using TraderApi.WebSocketManager;

namespace TraderApi.BinanceTradeManager
{
    public class TimerService : ITimerService
    {
        public List<BinancePair> timer { get; set; }
        public List<DataOfRealTimeRequest> data { get; set; }
        public IServiceProvider Services { get; }
        private NotificationsMessageHandler NotificationsService { get; set; }

        public TimerService(IServiceProvider services, NotificationsMessageHandler notificationsService)
        {
            Services = services;
            NotificationsService = notificationsService;
            data = new List<DataOfRealTimeRequest>
            {
                new DataOfRealTimeRequest(),
                new DataOfRealTimeRequest(),
                new DataOfRealTimeRequest(),
                new DataOfRealTimeRequest(),
                new DataOfRealTimeRequest(),
                new DataOfRealTimeRequest(),
                new DataOfRealTimeRequest()
            };
            timer = new List<BinancePair>
            {
                 new BinancePair(Pairs.GVTBTC),
                 new BinancePair(Pairs.IOTXBTC),
                 new BinancePair(Pairs.STRATBTC),
                 new BinancePair(Pairs.XRPBTC),
                 new BinancePair(Pairs.WAVESBTC),
                 new BinancePair(Pairs.CMTBTC),
                 new BinancePair(Pairs.BTCUSDT)
            };
            timer.ForEach(x =>
            {
                if (x.BinancePairr == Pairs.GVTBTC)
                {
                    x.Elapsed += (obj, e) => Count(data[0], e);
                }
                else if (x.BinancePairr == Pairs.IOTXBTC)
                {
                    x.Elapsed += (obj, e) => Count(data[1], e);
                }
                else if (x.BinancePairr == Pairs.STRATBTC)
                {
                    x.Elapsed += (obj, e) => Count(data[2], e);
                }
                else if (x.BinancePairr == Pairs.XRPBTC)
                {
                    //data.Clear();
                    x.Elapsed += (obj, e) => Count(data[3], e);
                }
                else if (x.BinancePairr == Pairs.WAVESBTC)
                {
                    x.Elapsed += (obj, e) => Count(data[4], e);
                }
                else if (x.BinancePairr == Pairs.CMTBTC)
                {
                    x.Elapsed += (obj, e) => Count(data[5], e);
                }
                else if (x.BinancePairr == Pairs.BTCUSDT)
                {
                    x.Elapsed += (obj, e) => Count(data[6], e);
                }
            });

        }

        public void Count(object obj, System.Timers.ElapsedEventArgs e)
        {
            var data = (DataOfRealTimeRequest)obj;
            var id = data.SocketId;

            Console.WriteLine(data.Interval + " " + data.Pair + " " + data.Inaccuracy + " " + System.DateTime.Now);
            string Kline;
            var time = new DateTime(data.Time.Year, data.Time.Month, (data.Time.Day + 2));
            using (var scope = Services.CreateScope())
            {
                var QuotationsFive =
                  scope.ServiceProvider
                  .GetRequiredService<IQuotationFiveService>();

                var Data = QuotationsFive.GetAll().Where(x => x.Pair == data.Pair && x.Interval == data.Interval && x.Date < time).OrderByDescending(x => x.ID).Take(7).ToList();
                var DataRed = QuotationsFive.GetAll().Where(x => x.Pair == data.Pair && x.Interval == data.Interval && x.Date < time).OrderByDescending(x => x.ID).Take(7).ToList();

                List<string> kline = new List<string>();
                var response = new List<string>();

                kline.Clear();
                response.Clear();
                int sumPlus = 0;
                int k = 0;
                int kolvo = 0;


                for (int i = 0; i < 7; i++)
                {
                    if (Data[i].Close >= Data[i].Open)
                    {
                        Kline = "Green";
                        kline.Add(Kline);
                    }
                    else
                    {
                        Kline = "Red";
                        kline.Add(Kline);
                    }
                }
                for (int i = 0; i < 7;)
                {
                    try
                    {
                        if (k >= kline.Count - 1)
                        {
                            break;
                        }
                        if (kline[k + 1].StartsWith("Green") && kline[k].StartsWith("Green"))
                        {
                            if (Math.Abs(Data[i + 1].MaxPrice - Data[i].MaxPrice) <= data.Inaccuracy)
                            {
                                sumPlus++;
                                i++;
                            }
                            else if ((Math.Abs(Data[i + 1].MaxPrice - Data[i].Close) <= data.Inaccuracy) && (Math.Abs(Data[i + 1].MaxPrice - Data[i].MaxPrice) <= data.Inaccuracy))
                            {
                                sumPlus++;
                                i++;
                            }
                            else
                            {
                                if (sumPlus > 0)
                                {
                                    Data.RemoveAt(0);
                                    for (int j = 0; j < sumPlus; j++)
                                    {
                                        Data.RemoveAt(0);
                                    }
                                    sumPlus = 0;
                                    i = 0;
                                }
                                else
                                {
                                    Data.RemoveAt(0);
                                    sumPlus = 0;
                                    i = 0;
                                }
                            }
                        }
                        else if (kline[k + 1].StartsWith("Red") && kline[k].StartsWith("Green"))
                        {
                            if (Math.Abs(Data[i + 1].MaxPrice - Data[i].MaxPrice) <= data.Inaccuracy)
                            {
                                sumPlus++;
                                i++;
                            }
                            else if ((Math.Abs(Data[i + 1].Open - Data[i].Close) <= data.Inaccuracy) && (Math.Abs(Data[i + 1].MaxPrice - Data[i].MaxPrice)) <= data.Inaccuracy)
                            {
                                sumPlus++;
                                i++;
                            }
                            else
                            {
                                if (sumPlus > 0)
                                {
                                    Data.RemoveAt(0);
                                    for (int j = 0; j < sumPlus; j++)
                                    {
                                        Data.RemoveAt(0);
                                    }
                                    sumPlus = 0;
                                    i = 0;
                                }
                                else
                                {
                                    Data.RemoveAt(0);
                                    sumPlus = 0;
                                    i = 0;
                                }
                            }
                        }
                        else if (kline[k + 1].StartsWith("Red") && kline[k].StartsWith("Red"))
                        {
                            if (Math.Abs(Data[i + 1].MaxPrice - Data[i].MaxPrice) <= data.Inaccuracy)
                            {
                                sumPlus++;
                                i++;
                            }
                            else if ((Math.Abs(Data[i + 1].MaxPrice - Data[i].Open) <= data.Inaccuracy) && (Math.Abs(Data[i + 1].MaxPrice - Data[i].MaxPrice) <= data.Inaccuracy))
                            {
                                sumPlus++;
                                i++;
                            }
                            else
                            {
                                if (sumPlus > 0)
                                {
                                    Data.RemoveAt(0);
                                    for (int j = 0; j < sumPlus; j++)
                                    {
                                        Data.RemoveAt(0);
                                    }
                                    sumPlus = 0;
                                    i = 0;
                                }
                                else
                                {
                                    Data.RemoveAt(0);
                                    sumPlus = 0;
                                    i = 0;
                                }
                            }
                        }
                        else if (kline[k + 1].StartsWith("Green") && kline[k].StartsWith("Red"))
                        {
                            if (Math.Abs(Data[i + 1].MaxPrice - Data[i].MaxPrice) <= data.Inaccuracy)
                            {
                                sumPlus++;
                                i++;
                            }
                            else if ((Math.Abs(Data[i + 1].Close - Data[i].Open) <= data.Inaccuracy) && ((Math.Abs(Data[i + 1].MaxPrice - Data[i].MaxPrice)) <= data.Inaccuracy))
                            {
                                sumPlus++;
                                i++;
                            }
                            else
                            {
                                if (sumPlus > 0)
                                {
                                    Data.RemoveAt(0);
                                    for (int j = 0; j < sumPlus; j++)
                                    {
                                        Data.RemoveAt(0);
                                    }
                                    sumPlus = 0;
                                    i = 0;
                                }
                                else
                                {
                                    Data.RemoveAt(0);
                                    sumPlus = 0;
                                    i = 0;
                                }
                            }
                        }
                        if (sumPlus == 3)
                        {
                            try
                            {
                                if ((kline[k + 2].StartsWith("Green") && kline[k + 1].StartsWith("Green") && Data[i].Close > Data[i + 1].Close)
                                    || (kline[k + 2].StartsWith("Red") && kline[k + 1].StartsWith("Green") && Data[i].Close > Data[i + 1].Open))
                                {
                                    if (kline[k + 3].StartsWith("Green") && Data[i].Close > Data[i + 2].Close)
                                    {
                                        if (kline[k + 4].StartsWith("Green") && Data[i].Close > Data[i + 3].Close)
                                        {
                                            var date = Data[i].Date + " Signal Up " + data.Pair;
                                            var socketId = NotificationsService.userID.GetValueOrDefault(data.Login);
                                            NotificationsService.NotifyToUser(socketId, date);

                                        }
                                        else if (kline[k + 4].StartsWith("Red") && Data[i].Close > Data[i + 3].Open)
                                        {
                                            var date = Data[i].Date + " Signal Up " + data.Pair;
                                            var socketId = NotificationsService.userID.GetValueOrDefault(data.Login);
                                            NotificationsService.NotifyToUser(socketId, date);
                                        }
                                    }
                                    else if (kline[k + 3].StartsWith("Red") && Data[i].Close > Data[i + 2].Open)
                                    {
                                        if (kline[k + 4].StartsWith("Green") && Data[i].Close > Data[i + 3].Close)
                                        {
                                            var date = Data[i].Date + " Signal Up " + data.Pair;
                                            var socketId = NotificationsService.userID.GetValueOrDefault(data.Login);
                                            NotificationsService.NotifyToUser(socketId, date);
                                        }
                                        else if (kline[k + 4].StartsWith("Red") && Data[i].Close > Data[i + 3].Open)
                                        {
                                            var date = Data[i].Date + " Signal Up " + data.Pair;
                                            var socketId = NotificationsService.userID.GetValueOrDefault(data.Login);
                                            NotificationsService.NotifyToUser(socketId, date);
                                        }
                                    }
                                }
                                else if ((kline[k + 2].StartsWith("Green") && kline[k + 1].StartsWith("Red") && Data[i].Open > Data[i + 1].Close)
                                        || (kline[k + 2].StartsWith("Red") && kline[k + 1].StartsWith("Red") && Data[i].Open > Data[i + 1].Open))
                                {
                                    if (kline[k + 3].StartsWith("Green") && Data[i].Open > Data[i + 2].Close)
                                    {
                                        if (kline[k + 4].StartsWith("Green") && Data[i].Open > Data[i + 3].Close)
                                        {
                                            var date = Data[i].Date + " Signal Up " + data.Pair;
                                            var socketId = NotificationsService.userID.GetValueOrDefault(data.Login);
                                            NotificationsService.NotifyToUser(socketId, date);
                                        }
                                        else if (kline[k + 4].StartsWith("Red") && Data[i].Open > Data[i + 3].Open)
                                        {
                                            var date = Data[i].Date + " Signal Up " + data.Pair;
                                            var socketId = NotificationsService.userID.GetValueOrDefault(data.Login);
                                            NotificationsService.NotifyToUser(socketId, date);
                                        }
                                    }
                                    else if (kline[k + 3].StartsWith("Red") && Data[i].Open > Data[i + 2].Open)
                                    {
                                        if (kline[k + 4].StartsWith("Green") && Data[i].Open > Data[i + 3].Close)
                                        {
                                            var date = Data[i].Date + " Signal Up " + data.Pair;
                                            var socketId = NotificationsService.userID.GetValueOrDefault(data.Login);
                                            NotificationsService.NotifyToUser(socketId, date);
                                        }
                                        else if (kline[k + 4].StartsWith("Red") && Data[i].Open > Data[i + 3].Open)
                                        {
                                            var date = Data[i].Date + " Signal Up " + data.Pair;
                                            var socketId = NotificationsService.userID.GetValueOrDefault(data.Login);
                                            NotificationsService.NotifyToUser(socketId, date);
                                        }
                                    }
                                }


                                kolvo++;
                                k++;
                                i = 0;

                                Data.RemoveAt(0);

                                for (int g = 0; g < sumPlus; g++)
                                {
                                    Data.RemoveAt(0);
                                }
                                sumPlus = 0;
                            }
                            catch
                            {
                                break;
                            }
                        }
                        k++;
                    }
                    catch
                    {
                        break;
                    }
                }
                k = 0;
                for (int i = 0; i < 7;)
                {
                    try
                    {
                        if (k >= kline.Count - 1)
                        {
                            break;
                        }
                        if (kline[k + 1].StartsWith("Green") && kline[k].StartsWith("Green"))
                        {
                            if (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].MinPrice) <= data.Inaccuracy)
                            {
                                sumPlus++;
                                i++;
                            }
                            else if (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].Open) <= data.Inaccuracy && (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].MinPrice) <= data.Inaccuracy))
                            {
                                sumPlus++;
                                i++;
                            }
                            else
                            {
                                if (sumPlus > 0)
                                {
                                    DataRed.RemoveAt(0);
                                    for (int j = 0; j < sumPlus; j++)
                                    {
                                        DataRed.RemoveAt(0);
                                    }
                                    sumPlus = 0;
                                    i = 0;
                                }
                                else
                                {
                                    DataRed.RemoveAt(0);
                                    sumPlus = 0;
                                    i = 0;
                                }
                            }
                        }
                        else if (kline[k + 1].StartsWith("Red") && kline[k].StartsWith("Green"))
                        {
                            if (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].MinPrice) <= data.Inaccuracy)
                            {
                                sumPlus++;
                                i++;
                            }
                            else if (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].Close) <= data.Inaccuracy && (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].MinPrice)) <= data.Inaccuracy)
                            {
                                sumPlus++;
                                i++;
                            }
                            else
                            {
                                if (sumPlus > 0)
                                {
                                    DataRed.RemoveAt(0);
                                    for (int j = 0; j < sumPlus; j++)
                                    {
                                        DataRed.RemoveAt(0);
                                    }
                                    sumPlus = 0;
                                    i = 0;
                                }
                                else
                                {
                                    DataRed.RemoveAt(0);
                                    sumPlus = 0;
                                    i = 0;
                                }
                            }
                        }
                        else if (kline[k + 1].StartsWith("Red") && kline[k].StartsWith("Red"))
                        {
                            if (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].MinPrice) <= data.Inaccuracy)
                            {
                                sumPlus++;
                                i++;
                            }
                            else if (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].Close) <= data.Inaccuracy && (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].MinPrice) <= data.Inaccuracy))
                            {
                                sumPlus++;
                                i++;
                            }
                            else
                            {
                                if (sumPlus > 0)
                                {
                                    DataRed.RemoveAt(0);
                                    for (int j = 0; j < sumPlus; j++)
                                    {
                                        DataRed.RemoveAt(0);
                                    }
                                    sumPlus = 0;
                                    i = 0;
                                }
                                else
                                {
                                    DataRed.RemoveAt(0);
                                    sumPlus = 0;
                                    i = 0;
                                }
                            }
                        }
                        else if (kline[k + 1].StartsWith("Green") && kline[k].StartsWith("Red"))
                        {
                            if (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].MinPrice) <= data.Inaccuracy)
                            {
                                sumPlus++;
                                i++;
                            }
                            else if (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].Close) <= data.Inaccuracy && (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].MinPrice)) <= data.Inaccuracy)
                            {
                                sumPlus++;
                                i++;
                            }
                            else
                            {
                                if (sumPlus > 0)
                                {
                                    DataRed.RemoveAt(0);
                                    for (int j = 0; j < sumPlus; j++)
                                    {
                                        DataRed.RemoveAt(0);
                                    }
                                    sumPlus = 0;
                                    i = 0;
                                }
                                else
                                {
                                    DataRed.RemoveAt(0);
                                    sumPlus = 0;
                                    i = 0;
                                }
                            }
                        }
                        if (sumPlus == 3)
                        {
                            try
                            {
                                if ((kline[k + 2].StartsWith("Green") && kline[k + 1].StartsWith("Green") && DataRed[i].Open < DataRed[i + 1].Open)
                                    || (kline[k + 2].StartsWith("Red") && kline[k + 1].StartsWith("Green") && DataRed[i].Open < DataRed[i + 1].Close))
                                {
                                    if (kline[k + 3].StartsWith("Green") && DataRed[i].Open < DataRed[i + 2].Close)
                                    {
                                        if (kline[k + 4].StartsWith("Green") && DataRed[i].Open < DataRed[i + 3].Close)
                                        {
                                            var date = Data[i].Date + " Signal Low " + data.Pair;
                                            var socketId = NotificationsService.userID.GetValueOrDefault(data.Login);
                                            NotificationsService.NotifyToUser(socketId, date);
                                        }
                                        else if (kline[k + 4].StartsWith("Red") && DataRed[i].Open < DataRed[i + 3].Close)
                                        {
                                            var date = Data[i].Date + " Signal Low " + data.Pair;
                                            var socketId = NotificationsService.userID.GetValueOrDefault(data.Login);
                                            NotificationsService.NotifyToUser(socketId, date);
                                        }
                                    }
                                    else if (kline[k + 3].StartsWith("Red") && DataRed[i].Open < DataRed[i + 2].Close)
                                    {
                                        if (kline[k + 4].StartsWith("Green") && DataRed[i].Open < DataRed[i + 3].Open)
                                        {
                                            var date = Data[i].Date + " Signal Low " + data.Pair;
                                            var socketId = NotificationsService.userID.GetValueOrDefault(data.Login);
                                            NotificationsService.NotifyToUser(socketId, date);
                                        }
                                        else if (kline[k + 4].StartsWith("Red") && DataRed[i].Open < DataRed[i + 3].Close)
                                        {
                                            var date = Data[i].Date + " Signal Low " + data.Pair;
                                            var socketId = NotificationsService.userID.GetValueOrDefault(data.Login);
                                            NotificationsService.NotifyToUser(socketId, date);
                                        }
                                    }
                                }
                                else if ((kline[k + 2].StartsWith("Green") && kline[k + 1].StartsWith("Red") && DataRed[i].Close < DataRed[i + 1].Open)
                                        || (kline[k + 2].StartsWith("Red") && kline[k + 1].StartsWith("Red") && DataRed[i].Close < DataRed[i + 1].Close))
                                {
                                    if (kline[k + 3].StartsWith("Green") && DataRed[i].Close < DataRed[i + 2].Open)
                                    {
                                        if (kline[k + 4].StartsWith("Green") && DataRed[i].Close < DataRed[i + 3].Open)
                                        {
                                            var date = Data[i].Date + " Signal Low " + data.Pair;
                                            var socketId = NotificationsService.userID.GetValueOrDefault(data.Login);
                                            NotificationsService.NotifyToUser(socketId, date);
                                        }
                                        else if (kline[k + 4].StartsWith("Red") && DataRed[i].Close < DataRed[i + 3].Close)
                                        {
                                            var date = Data[i].Date + " Signal Low " + data.Pair;
                                            var socketId = NotificationsService.userID.GetValueOrDefault(data.Login);
                                            NotificationsService.NotifyToUser(socketId, date);
                                        }
                                    }
                                    else if (kline[k + 3].StartsWith("Red") && DataRed[i].Close < DataRed[i + 2].Close)
                                    {
                                        if (kline[k + 4].StartsWith("Green") && DataRed[i].Close < DataRed[i + 3].Open)
                                        {
                                            var date = Data[i].Date + " Signal Low " + data.Pair;
                                            var socketId = NotificationsService.userID.GetValueOrDefault(data.Login);
                                            NotificationsService.NotifyToUser(socketId, date);
                                        }
                                        else if (kline[k + 4].StartsWith("Red") && DataRed[i].Close < DataRed[i + 3].Close)
                                        {
                                            var date = Data[i].Date + " Signal Low " + data.Pair;
                                            var socketId = NotificationsService.userID.GetValueOrDefault(data.Login);
                                            NotificationsService.NotifyToUser(socketId, date);
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                break;
                            }

                            kolvo++;
                            k++;
                            i = 0;

                            DataRed.RemoveAt(0);

                            for (int g = 0; g < sumPlus; g++)
                            {
                                DataRed.RemoveAt(0);
                            }

                            sumPlus = 0;
                        }
                        k++;
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            
            //return new KlineResponse { Success = true, Message = "succes" }); 
        }   
    }
}
