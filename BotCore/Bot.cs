using Discord.WebSocket;
using Discord;

namespace Bot
{
    public class Bot
    {
        private readonly DiscordSocketClient _client;
        public Bot()
        {
            DiscordSocketConfig discordSocketConfig = new DiscordSocketConfig
            {
                //setta a merda da intenção privilegiada
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            };

            _client = new DiscordSocketClient(discordSocketConfig);
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

            if (message.Content.Equals("salve"))
                await message.Channel.SendMessageAsync("vai se foder porra");

            if(message.Content.Equals("!oi"))
                await JoinVoice(message);
        }
        //TODO
        private async Task JoinVoice(SocketMessage message)
        {
            var voiceChannelId = (message.Author as IGuildUser).VoiceChannel.Id;
            var voiceChannel = _client.GetChannel(voiceChannelId) as IVoiceChannel;

            var audioClient = await voiceChannel.ConnectAsync();
        }
    }
}