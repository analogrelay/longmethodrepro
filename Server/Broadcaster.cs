using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Server
{
    public class Broadcaster : Hub<IBroadcaster>
    {
        public Task<string> GetUrl()
        {
            int counter = 0;

            return Task.Run(async () =>
            {
                for (int i = 0; i < 100; i++)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    await this.Clients.Client(this.Context.ConnectionId).SendProgress(counter, "Progress");
                    counter += 1;
                }

                return "url";
            });
        }
    }

    public interface IBroadcaster
    {
        Task SendProgress(double percent, string message);
    }
}