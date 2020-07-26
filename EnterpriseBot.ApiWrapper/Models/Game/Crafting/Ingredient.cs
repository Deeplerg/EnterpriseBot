namespace EnterpriseBot.ApiWrapper.Models.Game.Crafting
{
    public class Ingredient
    {
        public long Id { get; set; }
        public Item Item { get; set; }
        public int Quantity { get; set; }
        public Recipe Recipe { get; set; }
    }
}
