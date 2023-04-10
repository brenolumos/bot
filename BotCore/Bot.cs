using Discord.WebSocket;
using Discord;
using SharpLink;
using BotCore.Services;

namespace Bot
{
    public class Bot
    {
        private readonly DiscordSocketClient _client;
        private readonly LavalinkManager _lavalinkManager;
        private readonly MusicService _musicService;
        public Bot(LavalinkManager lavalinkManager, DiscordSocketClient client)
        {
            _client = client;
            _lavalinkManager = lavalinkManager;
            _client.Ready += async () =>
            {
                await _lavalinkManager.StartAsync();
            };

            _musicService = new MusicService(_lavalinkManager);

            _client.Log += LogAsync;
            _client.MessageReceived += OnMessageReceived;
        }

        public async Task RunAsync()
        {
            var token = "MTA4NjA1MzA1MjE4MTY0NzQ4MA.G4U4sG.HkzctQrP3SlJDIYJshaI2fHuZIDAh58O7E0P9A";

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private async Task OnMessageReceived(SocketMessage message)
        {
            if (message.Author.IsBot || message.Author.Id == _client.CurrentUser.Id)
            {
                return;
            }

            var firstChar = message.Content.Substring(0, 1);

            if (message.Content.Equals("salve"))
                await message.Channel.SendMessageAsync("vai se foder porra");

            if (firstChar == "!")
            {
                await _musicService.CommandsHandler(message);
            }
        }
    }
}