using EnterpriseBot.Api.Game.Business.Company;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Storages
{
    public class TrunkStorageCreationParams
    {
        public decimal Capacity { get; set; }

        public Truck OwningTruck { get; set; }
    }
}
