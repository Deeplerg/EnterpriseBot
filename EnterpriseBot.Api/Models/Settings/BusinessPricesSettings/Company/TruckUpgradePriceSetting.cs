using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.Settings.BusinessPricesSettings.Company
{
    public class TruckUpgradePriceSetting
    {
        public decimal Price { get; set; }
        public uint StepInSeconds { get; set; }
    }
}
