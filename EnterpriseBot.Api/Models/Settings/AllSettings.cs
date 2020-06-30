using EnterpriseBot.Api.Game.Donation;
using EnterpriseBot.Api.Models.Settings.BusinessPricesSettings;
using EnterpriseBot.Api.Models.Settings.GameplaySettings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.Settings
{
    public class AllSettings
    {
        public BusinessPricesSettings.BusinessPricesSettings BusinessPricesSettings { get; set; }
        public BusinessSettings.BusinessSettings BusinessSettings { get; set; }
        public GameplaySettings.GameplaySettings GameplaySettings { get; set; }
        public LocalizationSettings.LocalizationSettings LocalizationSettings { get; set; }
        public MarketSettings.MarketSettings MarketSettings { get; set; }
        public DonationSettings.DonationSettings DonationSettings { get; set; }
    }
}
