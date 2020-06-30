using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Game.Localization;
using System.Collections.Generic;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Crafting
{
    public class CraftingCategoryCreationParams
    {
        public LocalizedString Name { get; set; }
        public LocalizedString Description { get; set; }
        /// <summary>
        /// Optional
        /// </summary>
        public List<CraftingSubCategory> SubCategories { get; set; }
    }
}
