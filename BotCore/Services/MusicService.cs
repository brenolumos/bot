using SharpLink;
using Discord;
using Discord.WebSocket;
using System.Security.Cryptography;

namespace BotCore.Services
{
    public class MusicService
    {
        private readonly LavalinkManager _lavalinkManager;
        private LoadTracksResponse _response;
        private List<LavalinkTrack> _tracks;

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
            if ((message.Author as IGuildUser).VoiceChannel != player.VoiceChannel)
                await message.Channel.SendMessageAsync("Já estou conectado em outro canal de voz, maldito!");
            else
            {
                string[] content = message.Content.Split(" ");

                switch (content[0])
                {
                    case "!play":
                        await AddTrackAsync(content[1]);
                        break;
                    case "!pause":
                        await player.PauseAsync();
                        break;
                    case "!resume":
                        await player.ResumeAsync();
                        break;
                    case "!skip":
                        //TODO
                        break;
                    case "!leave":
                        await player.DisconnectAsync();
                        break;
                    default:
                        await message.Channel.SendMessageAsync("Comando não existe.");
                        return;
                }
            }
        }

        private async Task PlayRoutine(LavalinkPlayer player)
        {
            if (!player.Playing)
            {
                await player.PlayAsync(_tracks[0]);
                _tracks.RemoveAt(0);
            }
        }

        private async Task AddTrackAsync(string link)
        {
            _response = await _lavalinkManager.GetTracksAsync(link);

            if (_tracks == null)
                _tracks = new List<LavalinkTrack>(_response.Tracks);
            else
                _tracks.AddRange(_response.Tracks);
        }
    }
}
