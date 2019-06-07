using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Interfaces;
using TraderApi.Models.Request;
using Domain.Enum;
using TraderApi.ViewModels.Response;
using System.Collections.Generic;
using System.Linq;
using System;
using Binance.Net.Objects;
using TraderApi.Interface;

namespace TraderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RealController : Controller
    {
        private IQuotationFiveService QuotationsFive { get; set; }
        private ITimerService timer { get; set; }


        public RealController([FromServices] IQuotationFiveService quotations, ITimerService Timer)
        {
            QuotationsFive = quotations;
            timer = Timer;
        }

        [HttpPost("RealTime")]
        public async Task<IActionResult> RealTime([FromBody] DataOfRealTimeRequest request)
        {
            if (request.Pair == Pairs.GVTBTC)
            {
                timer.timer[0].Stop();
                timer.timer[0].Interval = 10000 * request.Value;
                timer.data[0] = request;
                timer.timer[0].Start();
            }
            else if (request.Pair == Pairs.IOTXBTC)
            {
                timer.timer[1].Stop();
                timer.timer[1].Interval = 10000 * request.Value;
                timer.data[1] = request;
                timer.timer[1].Start();
            }
            else if (request.Pair == Pairs.STRATBTC)
            {
                timer.timer[2].Stop();
                timer.timer[2].Interval = 10000 * request.Value;
                timer.data[2] = request;
                timer.timer[2].Start();
            }
            else if (request.Pair == Pairs.XRPBTC)
            {
                timer.timer[3].Stop();
                timer.timer[3].Interval = 10000 * request.Value;
                timer.data[3] = request;
                timer.timer[3].Start();
            }
            else if (request.Pair == Pairs.WAVESBTC)
            {
                timer.timer[4].Stop();
                timer.timer[4].Interval = 10000 * request.Value;
                timer.data[4] = request;
                timer.timer[4].Start();
            }
            else if (request.Pair == Pairs.CMTBTC)
            {
                timer.timer[5].Stop();
                timer.timer[5].Interval = 10000 * request.Value;
                timer.data[5] = request;
                timer.timer[5].Start();
            }
            else if (request.Pair == Pairs.BTCUSDT)
            {
                timer.timer[6].Stop();
                timer.timer[6].Interval = 10000 * request.Value;
                timer.data[6] = request;
                timer.timer[6].Start();
            }

            return Json(new KlineResponse { Success = true, });

        }
    }
}