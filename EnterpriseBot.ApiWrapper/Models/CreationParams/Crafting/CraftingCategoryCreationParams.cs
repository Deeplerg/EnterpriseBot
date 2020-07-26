using System.Collections.Generic;

namespace EnterpriseBot.ApiWrapper.Models.CreationParams.Crafting
{
    public class CraftingCategoryCreationParams
    {
        public long NameLocalizedStringId { get; set; }
        public long DescriptionLocalizedStringId { get; set; }

        /// <summary>
        /// Optional
        /// </summary>
        public List<long> CraftingSubCategoriesIds { get; set; }
    }
}
