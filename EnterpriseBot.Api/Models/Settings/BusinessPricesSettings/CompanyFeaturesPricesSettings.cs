namespace EnterpriseBot.Api.Models.Settings.BusinessPricesSettings.Company
{
    /// <summary>
    /// Represents prices for company features in business coins
    /// </summary>
    public class CompanyFeaturesPricesSettings
    {
        #region shop-specific extensions
        public decimal Base { get; set; }

        public decimal CanSignContracts { get; set; }

        public decimal CanUpgradeStorages { get; set; }
        public decimal CanBuyStorages { get; set; }
        public decimal CanHaveShowcase { get; set; }
        public StoragePricesSetting NewStoragePrices { get; set; }
        #endregion

        #region company-specific extensions
        public decimal CanHire { get; set; }

        public decimal CanHaveTruckGarage { get; set; }
        public decimal CanExtendTruckGarage { get; set; }
        public decimal NewTruckSetup { get; set; }

        public decimal CanHaveRobots { get; set; }
        public decimal NewRobotSetup { get; set; }
        public decimal CanExtendRobots { get; set; }
        public decimal CanUpgradeRobots { get; set; }

        public WorkerModifierUpgradePriceSetting WorkerModifierUpgrade { get; set; }
        public TruckUpgradePriceSetting TruckUpgrade { get; set; }
        public TruckGarageUpgradePriceSetting GarageUpgrade { get; set; }
        #endregion
    }
}
