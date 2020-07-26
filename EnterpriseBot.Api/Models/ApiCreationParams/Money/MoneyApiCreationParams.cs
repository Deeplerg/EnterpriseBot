using EnterpriseBot.Api.Models.Common.Enums;

namespace EnterpriseBot.Api.Models.ApiCreationParams.Money
{
    public class MoneyApiCreationParams
    {
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
    }
}
