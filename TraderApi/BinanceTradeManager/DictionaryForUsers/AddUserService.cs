using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TraderApi.Interface;
using TraderApi.Models.Request;

namespace TraderApi.BinanceTradeManager.DictionaryForUsers
{
    public class AddUserService : IAddUserService
    {
        private ITimerService TimerService { get; set; }
        private List<CurrentUser> users = new List<CurrentUser>();
        //private CurrentUser user = new CurrentUser();

        public AddUserService(ITimerService timerService)
        {
            TimerService = timerService;
        }

        public void AddUser(DataOfRealTimeRequest data)
        {
            CurrentUser user = new CurrentUser();
            user.id = data.UserId;
            user.User = data.Login;
            user.timer = new System.Timers.Timer();

            user.timer.Interval = 60000 * data.Value;
            user.timer.Elapsed += (obj, e) => TimerService.Count(data, e);
            user.timer.Start();



            users.Add(user);
        }
        public void DeleteUser(int UserId, string UserName)
        {
            var l = users.SingleOrDefault(x => x.id == UserId && x.User == UserName);
            l.timer.Stop();
            users.Remove(l);
        }
    }
   
}