using EnterpriseBot.ApiWrapper.Models.Enums;
using System.Collections.Generic;

namespace EnterpriseBot.ApiWrapper.Models.CreationParams.Crafting
{
    public class RecipeCreationParams
    {
        public List<long> IngredientsIds { get; set; }

        public long ResultItemId { get; set; }
        public short ResultItemQuantity { get; set; }

        public int LeadTimeInSeconds { get; set; }

        public RecipeCanBeDoneBy CanBeDoneBy { get; set; }
    }
}
