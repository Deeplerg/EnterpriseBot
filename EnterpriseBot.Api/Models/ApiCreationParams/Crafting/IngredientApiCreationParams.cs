namespace EnterpriseBot.Api.Models.ApiCreationParams.Crafting
{
    public class IngredientApiCreationParams
    {
        public long ItemId { get; set; }
        public int Quantity { get; set; }

        public long RecipeId { get; set; }
    }
}
