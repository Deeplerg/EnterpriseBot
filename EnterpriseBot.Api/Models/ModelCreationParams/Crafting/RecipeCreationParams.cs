using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Models.Common.Enums;
using System.Collections.Generic;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Crafting
{
    public class RecipeCreationParams
    {
        public List<Ingredient> Ingredients { get; set; }

        public Item ResultItem { get; set; }
        public short ResultItemQuantity { get; set; }

        public int LeadTimeInSeconds { get; set; }

        public RecipeCanBeDoneBy CanBeDoneBy { get; set; }
    }
}
