using System.Threading.Tasks;
using advanced_programming_2.Models;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs
{
    public class myHub:Hub
    {
        public async Task SendMessage()
        {
            await Clients.All.SendAsync("ReceiveMessage");
        }
    }
}
