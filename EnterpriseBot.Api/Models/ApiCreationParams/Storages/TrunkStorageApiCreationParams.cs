namespace EnterpriseBot.Api.Models.ApiCreationParams.Storages
{
    public class TrunkStorageApiCreationParams
    {
        public decimal Capacity { get; set; }

        public long OwningTruckId { get; set; }
    }
}
