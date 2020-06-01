namespace EnterpriseBot.Api.Models.Common.Crafting
{
    public class Ingredient
    {
        public long Id { get; set; }

        public long ItemId { get; set; }
        public virtual Item Item { get; set; }

        public int Quantity { get; set; }
    }
}
