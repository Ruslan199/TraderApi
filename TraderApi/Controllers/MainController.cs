using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Binance.Net;
using TraderApi.Models.Response;
using Domain;
using BusinessLogic.Interfaces;
using TraderApi.Models.Request;
using Utils;
using Domain.Enum;
using TraderApi.ViewModels.Response;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TraderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : Controller
    {

        private IQuotationFiveService QuotationsFive { get; set; }

        public MainController([FromServices] IQuotationFiveService quotations)
        {
            QuotationsFive = quotations;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PriceListRequest request)
        {
            var client = new BinanceClient();
            client.SetApiCredentials("7ICQp7LXtOdaFOTYyQV4GqjA2nwOzkwgOW3KgHCQTj5fyZHXNHD4XmRVW3BukcXZ", "MFr1NGf1rLwSaNZ5pOfndWG4GX4vIRVUBpiCXqckKhj44rBAjracQb44M1n0Ra7g");

            var requestHours = await client.Get24HPriceAsync(request.Pair);

            var requestKlines = await client.GetKlinesAsync(request.Pair, request.Interval, null, null, 1);

            decimal volume = requestKlines.Data[0].Volume;
            decimal volumeBTC = requestKlines.Data[0].QuoteAssetVolume;

            decimal percent = RequestHelper.CalculatePercent(requestKlines.Data);

            decimal priceChangePercent = requestHours.Data.PriceChangePercent;
            decimal quoteVolume = requestHours.Data.QuoteVolume;

            if (requestHours.Success && requestKlines.Success)
                return Json(new Binance24HPricesListResponse
                {
                    Success = true,
                    Volume = volume,
                    VolumeBTC = volumeBTC,
                    Percent = percent,
                    PriceChangePercent = priceChangePercent,
                    QuoteVolume = quoteVolume
                });



            return Json(new ResponseModel { Success = false, Message = "dsf" });
        }

        [HttpPost("GetData")]
        public async Task<IActionResult> GetData([FromBody] GetDataRequest request)
        {
            var example = QuotationsFive.GetAll().Where(x => x.Pair == request.Pair && x.Interval == request.Interval && x.Date >= request.StartTime.ToLocalTime() && x.Date <= request.EndTime.ToLocalTime()).ToArray();
            return Json(new DocumentResponse { Success = true, DataPrice = example });
        }

        [HttpPost("StartAlgoritm")]
        public async Task<IActionResult> StartAlgoritm([FromBody] DataOfAlgoritmRequest request)
        {
            var time = new DateTime(request.Time.Year, request.Time.Month, (request.Time.Day + 2));
            var inaccuracy = request.Inaccuracy;

            string Kline;

            var Data = QuotationsFive.GetAll().Where(x => x.Pair == request.Pair && x.Interval == request.Interval && x.Date < time).OrderByDescending(x=>x.ID).Take(request.KlinesCount).ToList();
            var DataRed = QuotationsFive.GetAll().Where(x => x.Pair == request.Pair && x.Interval == request.Interval && x.Date < time).OrderByDescending(x => x.ID).Take(request.KlinesCount).ToList();

            List<string> kline = new List<string>();
            kline.Clear();
            int sumPlus = 0;
            int k = 0;
            int kolvo = 0;


            for (int i = 0; i < request.KlinesCount; i++)
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
            for (int i = 0; i < request.KlinesCount; )
            {
                try
                {
                    if (k >= kline.Count - 1)
                    {
                        break;
                    }
                    if (kline[k + 1].StartsWith("Green") && kline[k].StartsWith("Green"))
                    {
                        if (Math.Abs(Data[i + 1].MaxPrice - Data[i].MaxPrice) <= inaccuracy)
                        {
                            sumPlus++;
                            i++;
                        }
                        else if ((Math.Abs(Data[i + 1].MaxPrice - Data[i].Close) <= inaccuracy) && (Math.Abs(Data[i + 1].MaxPrice - Data[i].MaxPrice) <= inaccuracy))
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
                        if (Math.Abs(Data[i + 1].MaxPrice - Data[i].MaxPrice) <= inaccuracy)
                        {
                            sumPlus++;
                            i++;
                        }
                        else if ((Math.Abs(Data[i + 1].Open - Data[i].Close) <= inaccuracy) && (Math.Abs(Data[i + 1].MaxPrice - Data[i].MaxPrice)) <= inaccuracy)
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
                        if (Math.Abs(Data[i + 1].MaxPrice - Data[i].MaxPrice) <= inaccuracy)
                        {
                            sumPlus++;
                            i++;
                        }
                        else if ((Math.Abs(Data[i + 1].MaxPrice - Data[i].Open) <= inaccuracy) && (Math.Abs(Data[i + 1].MaxPrice - Data[i].MaxPrice)  <= inaccuracy))
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
                        if (Math.Abs(Data[i + 1].MaxPrice - Data[i].MaxPrice) <= inaccuracy)
                        {
                            sumPlus++;
                            i++;
                        }
                        else if ((Math.Abs(Data[i + 1].Close - Data[i].Open) <= inaccuracy) && ((Math.Abs(Data[i + 1].MaxPrice - Data[i].MaxPrice)) <= inaccuracy))
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
                            if ((kline[k + 2].StartsWith("Green") && kline[k + 1].StartsWith("Green")&& Data[i].Close > Data[i + 1].Close)
                                || (kline[k + 2].StartsWith("Red") && kline[k + 1].StartsWith("Green") && Data[i].Close > Data[i + 1].Open))
                            {
                                if (kline[k + 3].StartsWith("Green") && Data[i].Close > Data[i + 2].Close)
                                {
                                    if (kline[k + 4].StartsWith("Green") && Data[i].Close > Data[i + 3].Close)
                                    {
                                        var date = Data[i].Date + " Сигнал Сверху " + request.Pair;
       
                                    }
                                    else if (kline[k + 4].StartsWith("Red") && Data[i].Close > Data[i + 3].Open)
                                    {
                                        var date = Data[i].Date + " Сигнал Сверху " + request.Pair;
                                       
                                    }
                                }
                                else if (kline[k + 3].StartsWith("Red") && Data[i].Close > Data[i + 2].Open)
                                {
                                    if (kline[k + 4].StartsWith("Green") && Data[i].Close > Data[i + 3].Close)
                                    {
                                        var date = Data[i].Date + " Сигнал Сверху " + request.Pair;
                                        
                                    }
                                    else if (kline[k + 4].StartsWith("Red") && Data[i].Close > Data[i + 3].Open)
                                    {
                                        var date = Data[i].Date + " Сигнал Сверху " + request.Pair;
                                       
                                    }
                                }
                            }
                            else if ((kline[k + 2].StartsWith("Green") && kline[k + 1].StartsWith("Red") && Data[i].Open > Data[i + 1].Close)
                                    || (kline[k + 2].StartsWith("Red") && kline[k + 1].StartsWith("Red") && Data[i].Open > Data[i + 1].Open ))
                                {
                                if (kline[k + 3].StartsWith("Green") && Data[i].Open > Data[i + 2].Close)
                                {
                                    if (kline[k + 4].StartsWith("Green") && Data[i].Open > Data[i + 3].Close)
                                    {
                                        var date = Data[i].Date + " Сигнал Сверху " + request.Pair;
                                       
                                    }
                                    else if (kline[k + 4].StartsWith("Red") && Data[i].Open > Data[i + 3].Open)
                                    {
                                        var date = Data[i].Date + " Сигнал Сверху " + request.Pair;
                                      
                                    }
                                }
                                else if (kline[k + 3].StartsWith("Red") && Data[i].Open > Data[i + 2].Open)
                                {
                                    if (kline[k + 4].StartsWith("Green") && Data[i].Open > Data[i + 3].Close)
                                    {
                                        var date = Data[i].Date + " Сигнал Сверху " + request.Pair;
                                        
                                    }
                                    else if (kline[k + 4].StartsWith("Red") && Data[i].Open > Data[i + 3].Open)
                                    {
                                        var date = Data[i].Date + " Сигнал Сверху " + request.Pair;
                                       
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
            for (int i = 0; i < request.KlinesCount;)
            {
                try
                {
                    if (k >= kline.Count - 1)
                    {
                        break;
                    }
                    if (kline[k + 1].StartsWith("Green") && kline[k].StartsWith("Green"))
                    {
                        if (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].MinPrice) <= inaccuracy)
                        {
                            sumPlus++;
                            i++;
                        }
                        else if (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].Open) <= inaccuracy && (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].MinPrice) <= inaccuracy))
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
                        if (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].MinPrice) <= inaccuracy)
                        {
                            sumPlus++;
                            i++;
                        }
                        else if (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].Close) <= inaccuracy && (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].MinPrice)) <= inaccuracy)
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
                        if (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].MinPrice) <= inaccuracy)
                        {
                            sumPlus++;
                            i++;
                        }
                        else if (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].Close) <= inaccuracy && (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].MinPrice) <= inaccuracy))
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
                        if (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].MinPrice) <= inaccuracy)
                        {
                            sumPlus++;
                            i++;
                        }
                        else if (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].Close) <= inaccuracy && (Math.Abs(DataRed[i + 1].MinPrice - DataRed[i].MinPrice)) <= inaccuracy)
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
                                        var date = DataRed[i].Date + " Сигнал Снизу " + request.Pair;
                                       
                                    }
                                    else if (kline[k + 4].StartsWith("Red") && DataRed[i].Open < DataRed[i + 3].Close)
                                    {
                                        var date = DataRed[i].Date + " Сигнал Снизу " + request.Pair;
                                      
                                    }
                                }
                                else if (kline[k + 3].StartsWith("Red") && DataRed[i].Open < DataRed[i + 2].Close)
                                {
                                    if (kline[k + 4].StartsWith("Green") && DataRed[i].Open < DataRed[i + 3].Open)
                                    {
                                        var date = DataRed[i].Date + " Сигнал Снизу " + request.Pair;
                                       
                                    }
                                    else if (kline[k + 4].StartsWith("Red") && DataRed[i].Open < DataRed[i + 3].Close)
                                    {
                                        var date = DataRed[i].Date + " Сигнал Снизу " + request.Pair;
                                       
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
                                        var date = DataRed[i].Date + " Сигнал Снизу " + request.Pair;
                                      
                                    }
                                    else if (kline[k + 4].StartsWith("Red") && DataRed[i].Close < DataRed[i + 3].Close)
                                    {
                                        var date = DataRed[i].Date + " Сигнал Снизу " + request.Pair;
                                      
                                    }
                                }
                                else if (kline[k + 3].StartsWith("Red") && DataRed[i].Close < DataRed[i + 2].Close)
                                {
                                    if (kline[k + 4].StartsWith("Green") && DataRed[i].Close < DataRed[i + 3].Open)
                                    {
                                        var date = DataRed[i].Date + " Сигнал Снизу " + request.Pair;
                                        
                                    }
                                    else if (kline[k + 4].StartsWith("Red") && DataRed[i].Close < DataRed[i + 3].Close)
                                    {
                                        var date = DataRed[i].Date + " Сигнал Снизу " + request.Pair;
                                       
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
            return Json(new KlineResponse { Success = true, });
            //to array();
        }
    }
}

