using Discord.WebSocket;
using Discord;
using SharpLink;

namespace Bot
{
    public class Bot
    {
        private readonly DiscordSocketClient _client;
        private readonly LavalinkManager _lavalinkManager;
        public Bot(LavalinkManager lavalinkManager, DiscordSocketClient client)
        {
            _client = client;
            _lavalinkManager = lavalinkManager;

            _client.Log += LogAsync;
            _client.Ready += async () =>
            {
                await _lavalinkManager.StartAsync();
            };
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

            if (message.Content.Equals("salve"))
                await message.Channel.SendMessageAsync("vai se foder porra");
            
            if (message.Content.Contains("!play"))
            {
                string[] conteudo = message.Content.Split(" ");

                //TODO CORTAR STRING PRA PEGAR O LINK DO VIDEO
                LavalinkPlayer player = _lavalinkManager.GetPlayer((message.Author as IGuildUser).Guild.Id) ?? await _lavalinkManager.JoinAsync((message.Author as IGuildUser).VoiceChannel);

                LoadTracksResponse response = await _lavalinkManager.GetTracksAsync(conteudo[1]);
                LavalinkTrack track = response.Tracks.First();
                await player.PlayAsync(track);
            }
        }
    }
}