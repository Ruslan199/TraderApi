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
using TraderApi.WebSocketManager;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using BusinessLogic;
using Microsoft.IdentityModel.Tokens;
using TraderApi.Models.Response;
using System.Net.Sockets;

namespace TraderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private IUserService UserService { get; set; }
        private IActivationKeyService ActivationKeyService { get; set; }
        private NotificationsMessageHandler NotificationsMessageHandlerService { get; set; }

        public UserController([FromServices]
            IUserService usersService,
            IActivationKeyService activationKeyService,
            NotificationsMessageHandler notificationsMessageHandler
            )
        {
            UserService = usersService;
            ActivationKeyService = activationKeyService;
            NotificationsMessageHandlerService = notificationsMessageHandler;
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
            

            return Json(new KlineResponse { Success = true, });
        }
        [HttpPost("authorization")]
        public async Task<IActionResult> Authorization([FromBody] AuthRequest request)
        {
            //try
           // {
                var user = UserService.GetAll().Where(x => x.Password == request.Password && x.Login == request.Login).SingleOrDefault().ID;

                var identity = GetIdentity(request.Login, request.Password);

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

            var l = NotificationsMessageHandlerService;


            return Json(new AuthResponse { Success = true, Message = encodedJwt });
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