using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Game.Localization;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Crafting
{
    public class ItemCreationParams
    {
        public LocalizedString Name { get; set; }

        public CraftingSubCategory Category { get; set; }

        public decimal Space { get; set; }
    }
}
