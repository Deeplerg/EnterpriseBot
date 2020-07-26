using EnterpriseBot.ApiWrapper.Models.Game.Localization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

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
