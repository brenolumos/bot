using BotCore.Services;
using Discord;
using Discord.WebSocket;
using SharpLink;

namespace Bot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            DiscordSocketConfig discordSocketConfig = new()
            {
                //setta a merda da intenção privilegiada
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            };

            DiscordSocketClient client = new(discordSocketConfig);
            LavalinkManager lavalinkManager = new LavalinkManager(client, new LavalinkManagerConfig
            {
                RESTHost = "localhost",
                RESTPort = 2333,
                WebSocketHost = "localhost",
                WebSocketPort = 2333,
                Authorization = "youshallnotpass",
                TotalShards = 1
            });
            
            var bot = new Bot(lavalinkManager, client);
            await bot.RunAsync();
        }
    }
}
