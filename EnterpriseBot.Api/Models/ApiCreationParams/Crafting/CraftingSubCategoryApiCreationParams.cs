using System.Collections.Generic;

namespace EnterpriseBot.Api.Models.ApiCreationParams.Crafting
{
    public class CraftingSubCategoryApiCreationParams
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
