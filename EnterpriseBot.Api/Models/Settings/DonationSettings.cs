using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.Settings.DonationSettings
{
    public class DonationSettings
    {
        public DonationBusinessPriceMultipliersSetting BusinessPriceMultipliers { get; set; }
        public DonationMaxContractsSetting MaxContracts { get; set; }
        public DonationContractMaxTimeSetting ContractMaxTimeInDays { get; set; }
    }
}
