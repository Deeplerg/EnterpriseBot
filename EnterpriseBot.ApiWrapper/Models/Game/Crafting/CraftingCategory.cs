using EnterpriseBot.ApiWrapper.Models.Game.Localization;
using System.Collections.Generic;

namespace EnterpriseBot.ApiWrapper.Models.Game.Crafting
{
    public class CraftingCategory
    {
        public long Id { get; set; }
        public LocalizedString Name { get; set; }
        public LocalizedString Description { get; set; }
        public List<CraftingSubCategory> SubCategories { get; set; } = new List<CraftingSubCategory>();
    }
}
