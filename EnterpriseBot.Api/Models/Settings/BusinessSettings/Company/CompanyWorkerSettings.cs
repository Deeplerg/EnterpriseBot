using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.Settings.BusinessSettings.Company
{
    public class CompanyWorkerSettings
    {
        public decimal MaxMultiplier { get; set; }
        public decimal DefaultMultiplier { get; set; }
        public uint DefaultItemsPerProduction { get; set; }
        public uint MaxItemsPerProduction { get; set; }
    }
}
