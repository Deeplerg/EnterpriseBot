using EnterpriseBot.Api.Game.Essences;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Storages
{
    public class InventoryStorageCreationParams
    {
        public Player OwningPlayer { get; set; }
        public decimal Capacity { get; set; }
    }
}
