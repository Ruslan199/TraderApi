using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Interfaces;
using TraderApi.Models.Request;
using TraderApi.ViewModels.Response;
using System.Collections.Generic;
using System.Linq;
using System;
using Domain;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using BusinessLogic;
using Microsoft.IdentityModel.Tokens;
using TraderApi.Models.Response;
using TraderApi.WebSocketManager;
using Microsoft.AspNetCore.Authorization;
using CryptoExchange.Net.Requests;

namespace TraderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private IUserService UserService { get; set; }
        private NotificationsMessageHandler NotificationsService { get; set; }

        public UserController([FromServices] IUserService usersService
            ,NotificationsMessageHandler notificationsService)
        {
            UserService = usersService;
            NotificationsService = notificationsService;
        }

        [HttpPost("registration")]
        public async Task<IActionResult> Registration([FromBody] UserRegistrationRequest request)
        {
            var now = DateTime.UtcNow;

            var users = UserService.GetAll().ToList();
            if (users.FirstOrDefault(x => x.Login == request.Login) != null)
               return Json(new KlineResponse { Success = false, Message = "This address already registered!" });

            var user = new User() {
                Login = request.Login,
                Mail = request.Mail,
                Password = request.Password
            };
            UserService.Create(user);
            

            return Json(new KlineResponse { Success = true, Message = "Вы успешно зарегестрировались"});
        }
        [Authorize]
        [HttpPost("exit")]
        public async Task<IActionResult> Exit([FromBody] ExitUserRequest request)
        {
            var user = UserService.GetAll().FirstOrDefault(x => x.Login == User.Identity.Name);
            var socketId = NotificationsService.userID.GetValueOrDefault(request.Login);

            if (socketId == null)
            {
                return Json(new KlineResponse { Success = false, Message = "К сожалению пользователь вышел" });
            }

            NotificationsService.RemoveSocketUser(user.Login, socketId);

            return Json(new KlineResponse { Success = true });
        }
        [HttpPost("CheckUser")]
        public async Task<IActionResult> CheckUser([FromBody] CheckCurrentUserRequest request)
        {
            var currenUser = NotificationsService.userID.FirstOrDefault(x=>x.Value == request.SocketId).Key;

            if (currenUser == null)
            {
                return Json(new KlineResponse { Success = false, Message = "Такого пользователя нет в сети" });
            }

            return Json(new KlineResponse { Success = true });
        }


        [HttpPost("authorization")]
        public async Task<IActionResult> Authorization([FromBody] AuthRequest request)
        {
            var user = UserService.GetAll().Where(x => x.Password == request.Password && x.Login == request.Login).SingleOrDefault();
            if (user == null)
            {
                return Json(new KlineResponse { Success = false, Message = "Неправильный логин или пароль" });
            }
            if (NotificationsService.userID.ContainsKey(request.Login))
            {
                return Json(new KlineResponse { Success = false, Message = "Пользователь уже в сети" });
            }

            var identity = GetIdentity(request.Login, request.Password);
            NotificationsService.addSocketUser(user.Login, request.SocketId);
            var now = DateTime.UtcNow;
            var expires = now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME));

            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: expires,

                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return Json(new AuthResponse { Success = true, Message = encodedJwt, Login = request.Login });
        }

        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        private ClaimsIdentity GetIdentity(string login, string password)
        {
            var user = UserService.GetAll().FirstOrDefault(x => x.Login == login && x.Password == password);

            if (user == null)
                return null;

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            return claimsIdentity;
        }
    }
}