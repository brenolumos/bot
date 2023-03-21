using Discord.WebSocket;

namespace Bot
{
    class Program
    {
        private readonly DiscordSocketClient _client;

        static async Task Main(string[] args)
        {
            var bot = new Bot();
            await bot.RunAsync();
        }
    }
}
