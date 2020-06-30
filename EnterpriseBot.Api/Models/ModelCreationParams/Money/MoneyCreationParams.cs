using EnterpriseBot.Api.Models.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
