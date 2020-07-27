using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.ModelCreationParams.Money;
using EnterpriseBot.Api.Models.Other;

namespace EnterpriseBot.Api.Game.Money
{
    public class Money
    {
        protected Money() { }

        #region model
        public decimal Amount { get; protected set; }
        public Currency Currency { get; protected set; }

        #region errors
        private static readonly LocalizedError amountLowerOrEqualToZero = new LocalizedError
        {
            ErrorSeverity = ErrorSeverity.Normal,
            EnglishMessage = "Money amount cannot be lower than or equal to 0",
            RussianMessage = "Количество денег не может быть меньше или равно 0"
        };
        #endregion
        #endregion

        #region actions
        public static GameResult<Money> Create(MoneyCreationParams pars)
        {
            if (pars.Amount < 0)
            {
                return MoneyAmountLowerThan0Error(pars.Amount);
            }

            return new Money
            {
                Amount = pars.Amount,
                Currency = pars.Currency
            };
        }

        public GameResult<decimal> Add(decimal amount)
        {
            if (amount <= 0)
            {
                return amountLowerOrEqualToZero;
            }

            Amount += amount;
            return Amount;
        }

        public GameResult<decimal> Reduce(decimal amount)
        {
            if (amount <= 0)
            {
                return amountLowerOrEqualToZero;
            }

            if (Amount < amount)
            {
                decimal needed = amount - Amount;

                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = $"Not enough money: {needed} more is needed",
                    RussianMessage = $"Недостаточно денег. Необходимо ещё {needed}"
                };
            }

            Amount -= amount;
            return Amount;
        }


        private static LocalizedError MoneyAmountLowerThan0Error(decimal butWas)
        {
            return new LocalizedError
            {
                ErrorSeverity = ErrorSeverity.Critical,
                EnglishMessage = $"Money amount cannot be lower than 0, but was: {butWas}",
                RussianMessage = $"Количество денег не может быть ниже 0, но было: {butWas}"
            };
        }
        #endregion
    }
}
