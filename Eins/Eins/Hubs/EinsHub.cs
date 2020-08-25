using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Eins.Hubs
{
    public class EinsHub:Hub
    {
        //TODO: Build the card and game logic here?
        public async Task SendMessage(string user, string message)
        {
            
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}