using EnterpriseBot.Api.Models.Common.Enums;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Money
{
    public class MoneyCreationParams
    {
        public MoneyCreationParams() { }

        public MoneyCreationParams(decimal amount, Currency currency)
        {
            this.Amount = amount;
            this.Currency = currency;
        }

        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
    }
}
