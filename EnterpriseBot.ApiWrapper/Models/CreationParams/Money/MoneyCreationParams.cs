using EnterpriseBot.ApiWrapper.Models.Enums;

namespace EnterpriseBot.ApiWrapper.Models.CreationParams.Money
{
    public class MoneyCreationParams
    {
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
    }
}
