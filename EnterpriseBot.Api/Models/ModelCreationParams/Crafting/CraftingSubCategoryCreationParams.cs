using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Game.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Crafting
{
    public class CraftingSubCategoryCreationParams
    {
        public LocalizedString Name { get; set; }
        public LocalizedString Description { get; set; }
        public CraftingCategory MainCategory { get; set; }
        /// <summary>
        /// Optional
        /// </summary>
        public List<Item> Items { get; set; }
    }
}
