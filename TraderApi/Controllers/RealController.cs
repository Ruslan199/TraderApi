using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Interfaces;
using TraderApi.Models.Request;
using TraderApi.ViewModels.Response;
using System.Collections.Generic;
using System.Linq;
using TraderApi.Interface;
using TraderApi.WebSocketManager;
using Microsoft.AspNetCore.Authorization;

namespace TraderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RealController : Controller
    {
        private IQuotationFiveService QuotationsFive { get; set; }
        private ITimerService timer { get; set; }
        private IUserService UserService { get; set; }
        private IAddUserService AddUserService { get; set; }
        private NotificationsMessageHandler NotificationsService { get; set; }

        public RealController([FromServices]
            IQuotationFiveService quotations,
            ITimerService Timer,
            IUserService userService,
            IAddUserService addUserService,
            NotificationsMessageHandler notificationsService
            )
        {
            QuotationsFive = quotations;
            timer = Timer;
            UserService = userService;
            NotificationsService = notificationsService;
            AddUserService = addUserService;
            bool work = false;
        }

        [Authorize]
        [HttpPost("RealTime")]
        public async Task<IActionResult> RealTime([FromBody] DataOfRealTimeRequest request)
        {
            var user = UserService.GetAll().FirstOrDefault(x => x.Login == User.Identity.Name);

            AddUserService.AddUser(request);     
            return Json(new KlineResponse { Success = true, });
        }
        [Authorize]
        [HttpPost("DeleteUserTimer")]
        public async Task<IActionResult> DeleteUserTimer([FromBody] DeleteTimerUser request)
        { 
            AddUserService.DeleteUser(request.UserId,request.UserName);
            return Json(new KlineResponse { Success = true, });
        }
    }
}