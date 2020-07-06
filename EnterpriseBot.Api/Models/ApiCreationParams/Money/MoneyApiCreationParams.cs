using EnterpriseBot.Api.Models.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.ApiCreationParams.Money
{
    public class MoneyApiCreationParams
    {
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
    }
}
