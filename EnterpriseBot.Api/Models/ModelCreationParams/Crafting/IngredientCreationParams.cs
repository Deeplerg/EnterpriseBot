using EnterpriseBot.Api.Game.Crafting;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Crafting
{
    public class IngredientCreationParams
    {
        public Item Item { get; set; }
        public int Quantity { get; set; }
        public Recipe Recipe { get; set; }
    }
}
