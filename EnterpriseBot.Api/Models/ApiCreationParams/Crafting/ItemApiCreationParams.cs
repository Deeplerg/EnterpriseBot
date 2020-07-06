using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.ApiCreationParams.Crafting
{
    public class ItemApiCreationParams
    {
        public long NameLocalizedStringId { get; set; }

        public long CraftingSubCategoryId { get; set; }

        public decimal Space { get; set; }
    }
}
