namespace EnterpriseBot.ApiWrapper.Models.CreationParams.Storages
{
    public class TrunkStorageCreationParams
    {
        public decimal Capacity { get; set; }

        public long OwningTruckId { get; set; }
    }
}
