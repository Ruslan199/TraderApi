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
using Domain;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;
using System.Threading;

namespace TraderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private IUserService user { get; set; }

        public UserController([FromServices] IUserService User)
        {
            user = User;
        }

        [HttpPost("createUser")]
        public async Task<IActionResult> UserRegistration([FromBody] UserRegistrationRequest request)
        {
            var users = new User() {
                UserName = request.UserName,
                Mail = request.Mail,
                Password = request.Password
            };
            user.Create(users);
            return Json(new KlineResponse { Success = true, });
        }
                CancellationToken.None);
        }
    }
}