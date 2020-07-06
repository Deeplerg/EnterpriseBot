using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.Settings
{
    public class GameSettings
    {
        public BusinessPricesSettings.BusinessPricesSettings BusinessPrices { get; set; }
        public BusinessSettings.BusinessSettings Business { get; set; }
        public DonationSettings.DonationSettings Donation { get; set; }
        public GameplaySettings.GameplaySettings Gameplay { get; set; }
        public LocalizationSettings.LocalizationSettings LocalizationSettings { get; set; }
        public MarketSettings.MarketSettings MarketSettings { get; set; }
    }
}
