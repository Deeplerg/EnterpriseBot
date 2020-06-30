using EnterpriseBot.Api.Game.Essences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Storages
{
    public class InventoryStorageCreationParams
    {
        public Player OwningPlayer { get; set; }
        public decimal Capacity { get; set; }
    }
}
