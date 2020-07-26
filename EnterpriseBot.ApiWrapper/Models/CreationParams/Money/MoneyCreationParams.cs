using EnterpriseBot.ApiWrapper.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Models.CreationParams.Money
{
    public class MoneyCreationParams
    {
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
    }
}
