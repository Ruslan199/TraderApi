using BusinessLogic.Interfaces;
using Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TraderApi.Interface;

namespace TraderApi.WebSocketManager
{
    public class NotificationsMessageHandler : WebSocketHandler
    {
        public List<SocketUser> socketUsers = new List<SocketUser>();
        
        public Dictionary<string, string> userID = new Dictionary<string, string>();

        public NotificationsMessageHandler(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
            
        }
        public void addSocketUser(string user, string socketId)
        {
            var socketID = WebSocketConnectionManager.GetSocketById(socketId);
            userID.Add(user, socketId);
        }
        public void RemoveSocketUser(string user, string sockterId)
        {
            userID.Remove(user,out sockterId);
            WebSocketConnectionManager.RemoveSocket(sockterId);
        }

        public void NotifyToUser(string user, string message)
        {
            var socketID = WebSocketConnectionManager.GetSocketById(user);
            SendMessageAsync(socketID, message);
        }

        public override Task OnConnected(WebSocket socket)
        {
            WebSocketConnectionManager.AddSocket(socket);
            var socketID = WebSocketConnectionManager.GetId(socket);  
            SendMessageAsync(socketID, socketID);
            return base.OnConnected(socket);
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var socketId = WebSocketConnectionManager.GetId(socket);
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            
            try
            {
                var recieveJSON = JsonConvert.DeserializeObject<SocketJSON>(message);

                    var existSocketUser = socketUsers.FirstOrDefault(x => x.ID.Equals(socketId));
                    if (existSocketUser == null)
                        socketUsers.Add(new SocketUser { ID = socketId });       
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }

    public class SocketJSON
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class SocketUser
    {
        public string ID { get; set; }
    }
}
