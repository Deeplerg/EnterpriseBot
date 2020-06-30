using EnterpriseBot.Api.Game.Crafting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Storages
{
    public class StorageItemCreationParams
    {
        public Item Item { get; set; }
        public int Quantity { get; set; }
    }
}
