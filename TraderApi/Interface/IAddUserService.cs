using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TraderApi.Models.Request;

namespace TraderApi.Interface
{
    public interface IAddUserService
    {
        void AddUser(DataOfRealTimeRequest data);
        void DeleteUser(int UserId,string UserName);
       // List<CurrentUser> users { get; set; }
       // CurrentUser user { get; set; }
    }
}
