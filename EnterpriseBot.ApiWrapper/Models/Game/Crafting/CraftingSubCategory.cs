using EnterpriseBot.ApiWrapper.Models.Game.Localization;
using System.Collections.Generic;

namespace EnterpriseBot.ApiWrapper.Models.Game.Crafting
{
    public class CraftingSubCategory
    {
        public long Id { get; set; }
        public LocalizedString Name { get; set; }
        public LocalizedString Description { get; set; }
        public CraftingCategory MainCategory { get; set; }
        public List<Item> Items { get; set; } = new List<Item>();
    }
}
