using Microsoft.AspNet.SignalR;

namespace SignalRChatApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly HubHandler _hubHandler;

        public ChatHub() : this(HubHandler.Instance) { }

        public ChatHub(HubHandler hubHandler)
        {
            _hubHandler = hubHandler;
        }
        public void Send(string name, string message)
        {
            // Call the addNewMessageToPage method to update clients
            //Clients.All.addNewMessageToPage(name, message);
            _hubHandler.Send(name, message);
        }
    }
}