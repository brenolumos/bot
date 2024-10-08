﻿using Discord.WebSocket;
using System.Text;
using BotCore.Validations;

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
            string? addOrSub = null;
            string[] numAndDiceType = content[0].Split('d', 'D');
            int.TryParse(numAndDiceType[0], out var numberOfRolls);
            int.TryParse(numAndDiceType[1], out var diceType);

            int[] rolls = new int[numberOfRolls];
            int rollTotal = 0;
            var sb = new StringBuilder();

            if (content.Length > 1)
            {
                addOrSub = content[1];
                extraValue = Int32.Parse(content[2]);
            }

            string? validationResult = await DiceRollerValidation.Validate(numberOfRolls, diceType, extraValue.GetValueOrDefault(), addOrSub);
            if (validationResult != null)
            {
                await message.Channel.SendMessageAsync(validationResult);
                return;
            }

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

            if (extraValue.HasValue)
            {
                if (addOrSub.Equals("+"))
                {
                    rollTotal += (int)extraValue;
                    sb.Append($" + ({extraValue})");
                } 
                else
                {
                    rollTotal -= (int)extraValue;
                    sb.Append($" - ({extraValue})");
                }
            }

            string response = $"Você rolou: {rollTotal}  (||" + sb.ToString() + "||)";
            await message.Channel.SendMessageAsync(response);
        }
    }
}
