﻿using Discord.WebSocket;
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
        private readonly TextService _textService;
        public Bot(LavalinkManager lavalinkManager, DiscordSocketClient client, TextService textService)
        {
            _client = client;
            _lavalinkManager = lavalinkManager;
            _textService = textService;
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
            //O bot foi excluído para que eu pudesse deixar o repositório público sem zoarem a minha token
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

            //Basicamente automatizei o bullying -> toda vez que meu querido amigo gustav mandava mensagem, meu bot reagia com emoji de nerd
            if (message.Author.Id == 681555613150347352) 
                await message.AddReactionAsync(new Emoji("🤓"));

            var firstChar = message.Content.Substring(0, 1);

            switch(firstChar)
            {
                case "!":
                    await _musicService.CommandsHandler(message);
                    break;
                case "%":
                    await _textService.CommandsHandler(message);
                    break;
            }
        }
    }
}