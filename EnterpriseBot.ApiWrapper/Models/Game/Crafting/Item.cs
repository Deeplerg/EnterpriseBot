using EnterpriseBot.ApiWrapper.Models.Game.Localization;

namespace EnterpriseBot.ApiWrapper.Models.Game.Crafting
{
    public class Item
    {
        public long Id { get; set; }
        public LocalizedString Name { get; set; }
        public CraftingSubCategory Category { get; set; }
        public decimal Space { get; set; } //how much space in the storage it takes
    }
}
