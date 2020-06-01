namespace EnterpriseBot.Api.Models.Settings.GameplaySettings //!!! Settings.GameplaySettings
{
    public class GameplaySettings
    {
        public PlayerGameplaySettings Player { get; set; }
        public CompanyGameplaySettings Company { get; set; }
        public ShopGameplaySettings Shop { get; set; }
        public JobGameplaySettings Job { get; set; }
        public TruckGameplaySettings Truck { get; set; }
        public TruckGarageGameplaySettings TruckGarage { get; set; }
        public StoragesGameplaySettings Storages { get; set; }
        public PricesGameplaySettings Prices { get; set; }
    }
}
