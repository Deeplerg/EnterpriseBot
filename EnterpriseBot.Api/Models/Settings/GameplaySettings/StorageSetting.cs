namespace EnterpriseBot.Api.Models.Settings.GameplaySettings
{
    public class StorageSetting
    {
        public decimal MaxCapacity { get; set; }
        public decimal DefaultCapacity { get; set; }
        public PriceSetting UpgradePrice { get; set; }
        public decimal UpgradeStep { get; set; }
    }
}
