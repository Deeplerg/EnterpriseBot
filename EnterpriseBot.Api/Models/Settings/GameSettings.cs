﻿namespace EnterpriseBot.Api.Models.Settings
{
    public class GameSettings
    {
        public BusinessPricesSettings.BusinessPricesSettings BusinessPrices { get; set; }
        public BusinessSettings.BusinessSettings Business { get; set; }
        public DonationSettings.DonationSettings Donation { get; set; }
        public GameplaySettings.GameplaySettings Gameplay { get; set; }
        public LocalizationSettings.LocalizationSettings Localization { get; set; }
        public MarketSettings.MarketSettings Market { get; set; }
    }
}
