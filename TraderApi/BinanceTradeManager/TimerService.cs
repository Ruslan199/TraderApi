﻿using BusinessLogic.Interfaces;
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
        public List<DataOfRealTimeRequest> data { get; set; }
        public IServiceProvider Services { get; }
        private NotificationsMessageHandler NotificationsService { get; set; }

        public TimerService(IServiceProvider services, NotificationsMessageHandler notificationsService)
        {
            Services = services;
            NotificationsService = notificationsService;
        }

        public void Count(object obj, System.Timers.ElapsedEventArgs e)
        {
            var data = (DataOfRealTimeRequest)obj;
            var id = data.SocketId;

            Console.WriteLine(data.Interval + " " + data.Pair + " " + data.Inaccuracy + " " + System.DateTime.Now + " " + data.Login);

            
            string Kline;
            var time = new DateTime(data.Time.Year, data.Time.Month, (data.Time.Day + 2));
            using (var scope = Services.CreateScope())
            {
                var QuotationsFive =
                  scope.ServiceProvider
                  .GetRequiredService<IQuotationFiveService>();

                var Data = QuotationsFive.GetAll().Where(x => x.Pair == data.Pair && x.Interval == data.Interval && x.Date < time).OrderByDescending(x => x.ID).Take(7).ToList();
                var DataRed = QuotationsFive.GetAll().Where(x => x.Pair == data.Pair && x.Interval == data.Interval && x.Date < time).OrderByDescending(x => x.ID).Take(7).ToList();

                var DataGetTime = QuotationsFive.GetAll().Where(x => x.Pair == data.Pair && x.Interval == data.Interval && x.Date < time).OrderByDescending(x => x.ID).Take(7).ToList();
                var DataRedGetTime = QuotationsFive.GetAll().Where(x => x.Pair == data.Pair && x.Interval == data.Interval && x.Date < time).OrderByDescending(x => x.ID).Take(7).ToList();
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
                                            var date = DataGetTime[7 - Data.Count].Date + " Signal Up " + data.Pair;
                                            var socketId = NotificationsService.userID.GetValueOrDefault(data.Login);
                                            NotificationsService.NotifyToUser(socketId, date);

                                        }
                                        else if (kline[k + 4].StartsWith("Red") && Data[i].Close > Data[i + 3].Open)
                                        {
                                            var date = DataGetTime[7 - Data.Count].Date + " Signal Up " + data.Pair;
                                            var socketId = NotificationsService.userID.GetValueOrDefault(data.Login);
                                            NotificationsService.NotifyToUser(socketId, date);
                                        }
                                    }
                                    else if (kline[k + 3].StartsWith("Red") && Data[i].Close > Data[i + 2].Open)
                                    {
                                        if (kline[k + 4].StartsWith("Green") && Data[i].Close > Data[i + 3].Close)
                                        {
                                            var date = DataGetTime[7 - Data.Count].Date + " Signal Up " + data.Pair;
                                            var socketId = NotificationsService.userID.GetValueOrDefault(data.Login);
                                            NotificationsService.NotifyToUser(socketId, date);
                                        }
                                        else if (kline[k + 4].StartsWith("Red") && Data[i].Close > Data[i + 3].Open)
                                        {
                                            var date = DataGetTime[7 - Data.Count].Date + " Signal Up " + data.Pair;
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
                                            var date = DataGetTime[7 - Data.Count].Date + " Signal Up " + data.Pair;
                                            var socketId = NotificationsService.userID.GetValueOrDefault(data.Login);
                                            NotificationsService.NotifyToUser(socketId, date);
                                        }
                                        else if (kline[k + 4].StartsWith("Red") && Data[i].Open > Data[i + 3].Open)
                                        {
                                            var date = DataGetTime[7 - Data.Count].Date + " Signal Up " + data.Pair;
                                            var socketId = NotificationsService.userID.GetValueOrDefault(data.Login);
                                            NotificationsService.NotifyToUser(socketId, date);
                                        }
                                    }
                                    else if (kline[k + 3].StartsWith("Red") && Data[i].Open > Data[i + 2].Open)
                                    {
                                        if (kline[k + 4].StartsWith("Green") && Data[i].Open > Data[i + 3].Close)
                                        {
                                            var date = DataGetTime[7 - Data.Count].Date + " Signal Up " + data.Pair;
                                            var socketId = NotificationsService.userID.GetValueOrDefault(data.Login);
                                            NotificationsService.NotifyToUser(socketId, date);
                                        }
                                        else if (kline[k + 4].StartsWith("Red") && Data[i].Open > Data[i + 3].Open)
                                        {
                                            var date = DataGetTime[7 - Data.Count].Date + " Signal Up " + data.Pair;
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
                                            var date = DataRedGetTime[7 - DataRed.Count].Date + " Signal Low " + data.Pair;
                                            var socketId = NotificationsService.userID.GetValueOrDefault(data.Login);
                                            NotificationsService.NotifyToUser(socketId, date);
                                        }
                                        else if (kline[k + 4].StartsWith("Red") && DataRed[i].Open < DataRed[i + 3].Close)
                                        {
                                            var date = DataRedGetTime[7 - DataRed.Count].Date + " Signal Low " + data.Pair;
                                            var socketId = NotificationsService.userID.GetValueOrDefault(data.Login);
                                            NotificationsService.NotifyToUser(socketId, date);
                                        }
                                    }
                                    else if (kline[k + 3].StartsWith("Red") && DataRed[i].Open < DataRed[i + 2].Close)
                                    {
                                        if (kline[k + 4].StartsWith("Green") && DataRed[i].Open < DataRed[i + 3].Open)
                                        {
                                            var date = DataRedGetTime[7 - DataRed.Count].Date + " Signal Low " + data.Pair;
                                            var socketId = NotificationsService.userID.GetValueOrDefault(data.Login);
                                            NotificationsService.NotifyToUser(socketId, date);
                                        }
                                        else if (kline[k + 4].StartsWith("Red") && DataRed[i].Open < DataRed[i + 3].Close)
                                        {
                                            var date = DataRedGetTime[7 - DataRed.Count].Date + " Signal Low " + data.Pair;
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
                                            var date = DataRedGetTime[7 - DataRed.Count].Date + " Signal Low " + data.Pair;
                                            var socketId = NotificationsService.userID.GetValueOrDefault(data.Login);
                                            NotificationsService.NotifyToUser(socketId, date);
                                        }
                                        else if (kline[k + 4].StartsWith("Red") && DataRed[i].Close < DataRed[i + 3].Close)
                                        {
                                            var date = DataRedGetTime[7 - DataRed.Count].Date + " Signal Low " + data.Pair;
                                            var socketId = NotificationsService.userID.GetValueOrDefault(data.Login);
                                            NotificationsService.NotifyToUser(socketId, date);
                                        }
                                    }
                                    else if (kline[k + 3].StartsWith("Red") && DataRed[i].Close < DataRed[i + 2].Close)
                                    {
                                        if (kline[k + 4].StartsWith("Green") && DataRed[i].Close < DataRed[i + 3].Open)
                                        {
                                            var date = DataRedGetTime[7 - DataRed.Count].Date + " Signal Low " + data.Pair;
                                            var socketId = NotificationsService.userID.GetValueOrDefault(data.Login);
                                            NotificationsService.NotifyToUser(socketId, date);
                                        }
                                        else if (kline[k + 4].StartsWith("Red") && DataRed[i].Close < DataRed[i + 3].Close)
                                        {
                                            var date = DataRedGetTime[7 - DataRed.Count].Date + " Signal Low " + data.Pair;
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
