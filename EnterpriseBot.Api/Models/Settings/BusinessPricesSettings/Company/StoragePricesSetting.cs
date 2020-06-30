using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.Settings.BusinessPricesSettings.Company
{
    public class StoragePricesSetting
    {
        public decimal Company { get; set; }
        public decimal Showcase { get; set; }
        public decimal Income { get; set; }
    }
}
