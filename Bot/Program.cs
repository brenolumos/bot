using Discord;
using Discord.WebSocket;

public class Bot
{
    private readonly DiscordSocketClient _client;

    public Bot(string token)
    {
        _client = new DiscordSocketClient();
        _client.LoginAsync(TokenType.Bot, token);
        _client.StartAsync();
    }
}
