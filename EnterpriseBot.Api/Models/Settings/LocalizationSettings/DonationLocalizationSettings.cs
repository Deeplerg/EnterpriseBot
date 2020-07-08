using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.Settings.LocalizationSettings
{
    public class DonationLocalizationSettings
    {
        public LocalizationSetting Donation { get; set; }
        public LocalizationSetting DonationPriceCurrency { get; set; }
        public LocalizationSetting DonationPurchase { get; set; }
        public LocalizationSetting Privilege { get; set; }
    }
}
