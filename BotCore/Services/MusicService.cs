using SharpLink;
using Discord;
using Discord.WebSocket;
namespace BotCore.Services
{
    public class MusicService
    {
        private readonly LavalinkManager _lavalinkManager;
        private LoadTracksResponse _response;
        private List<LavalinkTrack> _tracks;
        private ISocketMessageChannel? _channel;
        private bool _skip;


        public MusicService(LavalinkManager lavaLinkManager)
        {
            _lavalinkManager = lavaLinkManager;
        }

        public async Task CommandsHandler(SocketMessage message)
        {
            if ((message.Author as IGuildUser).VoiceChannel == null)
            {
                await message.Channel.SendMessageAsync("Você precisa estar conectado em um canal de voz para usar esse comando.");
                return;
            }

            //busca player ativo na guild, caso não haja, dá join no voice channel
            LavalinkPlayer player = _lavalinkManager.GetPlayer((message.Author as IGuildUser).Guild.Id) ??
                await _lavalinkManager.JoinAsync((message.Author as IGuildUser).VoiceChannel);

            _channel = message.Channel;

            if ((message.Author as IGuildUser).VoiceChannel != player.VoiceChannel && player.Playing)
                await message.Channel.SendMessageAsync("Já estou conectado em outro canal de voz, maldito!");
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
                        _skip = true;
                        await PlayRoutine(player, _skip);
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

        private async Task PlayRoutine(LavalinkPlayer player, bool skip = false)
        {
            if (_tracks.Count == 0)
                return;

            if (skip)
            {
                var nextTrack = _tracks.First();
                _tracks.RemoveAt(0);
                await player.StopAsync();
                await SendTextMessage("Tocando agora: " + nextTrack.Title);
                await player.PlayAsync(nextTrack);
                _skip = false;
            }
            else
            {
                if (!player.Playing)
                {
                    await player.PlayAsync(_tracks.First());
                    await SendTextMessage("Tocando agora: " + _tracks.First().Title);
                    _tracks.RemoveAt(0);
                }
                else
                {
                    while (player.Playing)
                    {
                        await Task.Delay(3000);
                    }
                    if (_tracks.Count > 0 && !_skip)
                        PlayRoutine(player);
                }
            }
        }

        private async Task SendTextMessage(string text)
        {
            await _channel.SendMessageAsync(text);
        }

        private async Task AddTrackAsync(LavalinkPlayer player, string link)
        {
            _response = await _lavalinkManager.GetTracksAsync(link);

            if (_tracks == null)
                _tracks = new List<LavalinkTrack>(_response.Tracks);
            else
                _tracks.AddRange(_response.Tracks);

            PlayRoutine(player);
        }

        private async Task DisconnectAsync(LavalinkPlayer player)
        {
            _tracks.Clear();
            _channel = null;
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
