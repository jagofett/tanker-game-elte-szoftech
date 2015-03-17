using System;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
namespace SignalRChatApp.Hubs
{
    public class HubHandler
    {
        private IHubContext _context;
        private readonly static Lazy<HubHandler> _instance = new Lazy<HubHandler>(() => new HubHandler(GlobalHost.ConnectionManager.GetHubContext<ChatHub>()));

        private HubHandler(IHubContext Context)
        {
            _context = Context;
        }
        public static HubHandler Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        public void Send(string name, string message)
        {
            // Call the addNewMessageToPage method to update clients
            _context.Clients.All.addNewMessageToPage(name, message);

        }

        public void LobbyList(Object userList)
        {
            _context.Clients.All.lobbyList(userList);
        }

        public void StartGame(Object userList)
        {
            _context.Clients.All.startGame(userList);
        }

    }
}