using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.Settings.BusinessPricesSettings.Company
{
    public class WorkerModifierUpgradePriceSetting
    {
        public decimal Price { get; set; }
        public decimal Step { get; set; }
    }
}
