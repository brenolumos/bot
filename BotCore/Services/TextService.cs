using Discord.WebSocket;
using System.Text;

namespace BotCore.Services
{
    public class TextService
    {
        private Random Random = new Random();
        public async Task CommandsHandler(SocketMessage message)
        {
            string[] input = message.Content.Split(" ");
            string commandTag = input[0];

            switch (commandTag)
            {
                case "%d":
                    await DiceRoller(message);
                    break;
                case "%help":
                    await BotHelpInfo(message);
                    break;
                default:
                    await message.Channel.SendMessageAsync("Comando inexistente.");
                    break;
            }
        }

        private async Task BotHelpInfo(SocketMessage message)
        {
            await message.Channel.SendMessageAsync("```" + "**Comandos**\r\n\r\n" +
                                                   "! - Serviço de Música\r\n\n" +
                                                   "!play - Adiciona uma faixa do youtube à fila de reprodução.\r\n" +
                                                   "!pause - Pausa a reprodução da faixa atual.\r\n" +
                                                   "!resume - Retoma a reprodução da faixa atual.\r\n" +
                                                   "!skip - Avança para a próxima faixa na fila de reprodução.\r\n" +
                                                   "!leave - Disconecta do canal de voz atual.\r\n" +
                                                   "!qinfo - Apresenta as faixas presentes na fila de reprodução.\r\n\r\n\n" +
                                                   "%d - Rolador de Dados\r\n\n" +
                                                   "Síntaxe: %d (quantidade de lançamentos)d(tipo de dado) +/-(sinal do modificador opcional) (valor do modificador opcional)\r\n" +
                                                   "Ex: %d 2d12 + 8 \n    %d 7d8 - 12 \n    %d 3d20\r\n\r\n\r\nFeito com muito ódio por: Metafisico#2156" + "```");
        }

        private async Task DiceRoller(SocketMessage message)
        {
            string[] input = message.Content.Split(" ");
            string[] content = input.Skip(1).ToArray();

            int? extraValue = null;
            string? signal = null;

            if (content.Length > 1)
            {
                signal = content[1];
                extraValue = Int32.Parse(content[2]);
            }
            string[] numAndDiceType = content[0].Split('d', 'D');
            int numberOfRolls = Int32.Parse(numAndDiceType[0]);

            if (numberOfRolls > 200)
            {
                await message.Channel.SendMessageAsync("Olha o cara que tem dado até dizer chega...");
                return;
            }

            int diceType = Int32.Parse(numAndDiceType[1]);

            int[] rolls = new int[numberOfRolls];
            int rollTotal = 0;
            var sb = new StringBuilder();

            for (int i = 0; i < numberOfRolls; i++)
            {
                rolls[i] = Random.Next(1, diceType + 1);
                rollTotal += rolls[i];

                if (diceType == 20)
                {
                    if (i == numberOfRolls - 1 && rolls[i] == 20)
                        sb.Append($"**{rolls[i]}**");
                    else if (i == numberOfRolls - 1 && rolls[i] == 1)
                        sb.Append($"*{rolls[i]}*");
                    else if (i == numberOfRolls - 1)
                        sb.Append($"{rolls[i]}");
                    else if (rolls[i] == 20)
                        sb.Append($"**{rolls[i]}** + ");
                    else if (rolls[i] == 1)
                        sb.Append($"*{rolls[i]}* + ");
                    else
                        sb.Append($"{rolls[i]} + ");
                }
                else
                {
                    if (i == numberOfRolls - 1)
                        sb.Append($"{rolls[i]}");
                    else
                        sb.Append($"{rolls[i]} + ");
                }
            }
            if (signal == "+" && extraValue != null)
            {
                rollTotal += (int)extraValue;
                sb.Append($" + ({extraValue})");
            }
            else if (signal == "-" && extraValue != null)
            {
                rollTotal -= (int)extraValue;
                sb.Append($" - ({extraValue})");
            }

            string response = $"Você rolou: {rollTotal}  (||" + sb.ToString() + "||)";
            await message.Channel.SendMessageAsync(response);
        }
    }
}
