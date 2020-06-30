using EnterpriseBot.Api.Game.Money;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.ModelCreationParams.Money;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.Settings
{
    public class PriceSetting
    {
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }

        public static implicit operator Money(PriceSetting price)
        {
            return Money.Create(new MoneyCreationParams
            {
                Amount = price.Amount,
                Currency = price.Currency
            });
        }
    }
}
