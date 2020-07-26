using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Models.CreationParams.Crafting
{
    public class CraftingSubCategoryCreationParams
    {
        public long NameLocalizedStringId { get; set; }
        public long DescriptionLocalizedStringId { get; set; }
        public long MainCraftingCategoryId { get; set; }

        /// <summary>
        /// Optional
        /// </summary>
        public List<long> ItemsIds { get; set; }
    }
}
