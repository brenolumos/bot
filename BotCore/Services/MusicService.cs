using SharpLink;
using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace BotCore.Services
{
    public class MusicService
    {
        private readonly LavalinkManager _lavalinkManager;
        private List<LavalinkTrack> _tracks;
        private ISocketMessageChannel _channel;
        private SocketMessage _message;
        private System.Timers.Timer _trackFinishedTimer;

        public MusicService(LavalinkManager lavalinkManager)
        {
            _lavalinkManager = lavalinkManager;
            _tracks = new List<LavalinkTrack>();
            _channel = null;
            _trackFinishedTimer = new System.Timers.Timer(3000); // Defina o intervalo adequado em milissegundos
            _trackFinishedTimer.Elapsed += TrackFinishedTimer_Elapsed;
        }

        public async Task CommandsHandler(SocketMessage message)
        {
            if (!(message.Author is IGuildUser guildUser))
            {
                await message.Channel.SendMessageAsync("Esse comando só pode ser executado em um servidor.");
                return;
            }

            if (guildUser.VoiceChannel == null)
            {
                await message.Channel.SendMessageAsync("Você precisa estar conectado a um canal de voz para usar esse comando.");
                return;
            }

            LavalinkPlayer player = _lavalinkManager.GetPlayer(guildUser.Guild.Id) ?? await _lavalinkManager.JoinAsync(guildUser.VoiceChannel);
            _channel = message.Channel;
            _message = message;


            if (guildUser.VoiceChannel != player.VoiceChannel && player.Playing)
            {
                await message.Channel.SendMessageAsync("Já estou conectado a outro canal de voz.");
            }
            else
            {
                string[] content = message.Content.Split(" ");

                switch (content[0])
                {
                    case "!play":
                        await AddTrackAsync(player, content[1]);
                        break;
                    case "!pause":
                        await player.PauseAsync();
                        break;
                    case "!resume":
                        await player.ResumeAsync();
                        break;
                    case "!skip":
                        await PlayNextTrack(player);
                        break;
                    case "!leave":
                        await DisconnectAsync(player);
                        break;
                    case "!qinfo":
                        await QueueInfo();
                        break;
                    default:
                        await message.Channel.SendMessageAsync("Comando inexistente.");
                        return;
                }
            }
        }

        private void TrackFinishedTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var guildID = (IGuildUser)_message.Author;



            var player = _lavalinkManager.GetPlayer(guildID.Guild.Id);
            if (!player.Playing)
                PlayNextTrack(player);
        }

        private async Task SendTextMessage(string text)
        {
            await _channel.SendMessageAsync(text);
        }

        private async Task AddTrackAsync(LavalinkPlayer player, string link)
        {
            var response = await _lavalinkManager.GetTracksAsync(link);

            if (_tracks == null)
                _tracks = new List<LavalinkTrack>(response.Tracks);
            else
                _tracks.AddRange(response.Tracks);

            if (!player.Playing)
                PlayNextTrack(player);
        }

        private async Task PlayNextTrack(LavalinkPlayer player)
        {
            if (_tracks.Count == 0)
                return;

            var nextTrack = _tracks[0];
            _tracks.RemoveAt(0);

            await player.StopAsync();
            await SendTextMessage("Tocando agora: " + nextTrack.Title);
            await player.PlayAsync(nextTrack);

            _trackFinishedTimer.Start();
        }

        private async Task DisconnectAsync(LavalinkPlayer player)
        {
            _tracks.Clear();
            _channel = null;
            _trackFinishedTimer.Stop();
            await player.DisconnectAsync();
        }

        private async Task QueueInfo()
        {
            string text = string.Empty;

            if (_tracks.Count == 0)
                text = "Não existem itens na fila de reprodução.";
            else
            {
                for (int i = 0; i < _tracks.Count; i++)
                {
                    string trackLength = $"{_tracks[i].Length.Minutes}m{_tracks[i].Length.Seconds}s";
                    text += $"{i + 1} - {_tracks[i].Title} ({trackLength})\n";
                }
            }

            await SendTextMessage(text);
        }
    }
}
