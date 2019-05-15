using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            if(args.Length != 1)
            {
                Console.Error.WriteLine($"Usage: {typeof(Program).Assembly.GetName().Name} <URL>");
                return 1;
            }
            var url = args[0];
            var connection = new HubConnectionBuilder()
                .WithUrl(args[0])
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Trace);
                    logging.AddConsole();
                })
                .Build();

            connection.On("SendProgress", (double percent, string message) =>
            {
                Console.WriteLine($"{percent,3:0}%: {message}\n");
            });

            Console.WriteLine("Starting connection...");
            await connection.StartAsync();
            Console.WriteLine("Invoking method...");
            var result = await connection.InvokeAsync<string>("GetUrl");
            Console.WriteLine($"Invoked method. Url: {result}");
            return 0;
        }
    }
}
