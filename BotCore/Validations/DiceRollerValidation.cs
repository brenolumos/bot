using Discord;

namespace BotCore.Validations
{
    public static class DiceRollerValidation
    {
        public static async Task<string?> Validate(int numberOfRolls, int diceType, int? extraValue = null, string? addOrSub = null)
        {
            string errors = string.Empty;
            int limiter;

            if (numberOfRolls > 200)
                errors += "Devido à limitação de caracteres do Discord, o número máximo de rolagens é 200.\n";

            if (numberOfRolls < 1)
                errors += "Número de rolagens inválido.\n";

            if (diceType < 2)
                errors += "Quantidade de lados inválida.\n";

            if (addOrSub != null && !addOrSub.Equals("+") && !addOrSub.Equals("-"))
                errors += "O modificador deve ser positivo (+) ou negativo (-).";

            checked
            {
                try
                {
                    limiter = numberOfRolls * diceType;
                    limiter = (numberOfRolls * diceType) + extraValue.GetValueOrDefault(0);
                    limiter = (numberOfRolls * diceType) - extraValue.GetValueOrDefault(0);
                }
                catch (OverflowException)
                {
                    errors += "Todos os valores utilizados pelo serviço de rolagem de dados são do tipo Int32 do C#. " +
                              "Quaisquer inputs que extrapolarem esse valor serão desconsiderados " + new Emoji("😶‍🌫️");
                }
            }

            if (errors == string.Empty)
            {
                await Task.CompletedTask;
                return null;
            }

            return await Task.FromResult(errors);

        }
    }
}
