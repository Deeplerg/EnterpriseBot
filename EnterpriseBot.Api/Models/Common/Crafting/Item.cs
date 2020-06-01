namespace EnterpriseBot.Api.Models.Common.Crafting
{
    public class Item
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public virtual CraftingCategory Category { get; set; }

        public int Space { get; set; } //how many space in the storage it takes
    }
}
