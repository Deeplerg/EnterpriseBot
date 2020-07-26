using EnterpriseBot.ApiWrapper.Models.Enums;
using System.Collections.Generic;

namespace EnterpriseBot.ApiWrapper.Models.Game.Crafting
{
    public class Recipe
    {
        public long Id { get; set; }
        public Item ResultItem { get; set; }
        public int ResultItemQuantity { get; set; }
        public int LeadTimeInSeconds { get; set; } //how much time in seconds it takes to produce it
        public RecipeCanBeDoneBy CanBeDoneBy { get; set; }
        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
    }
}
