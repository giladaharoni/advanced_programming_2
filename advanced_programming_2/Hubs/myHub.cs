using Microsoft.AspNetCore.SignalR;

namespace advanced_programming_2.Hubs
{
    public class myHub: Hub
    {
        public async Task Changed(string value)
        {
            await Clients.All.SendAsync("ChangedReceived", value);
        }

        public async Task newMess(string sender, string reciever)
        {
            await Clients.User(reciever).SendAsync(sender);
        }

    }
}
