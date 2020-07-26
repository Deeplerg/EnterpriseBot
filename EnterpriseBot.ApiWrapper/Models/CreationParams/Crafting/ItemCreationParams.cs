using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Models.CreationParams.Crafting
{
    public class ItemCreationParams
    {
        public long NameLocalizedStringId { get; set; }

        public long CraftingSubCategoryId { get; set; }

        public decimal Space { get; set; }
    }
}
