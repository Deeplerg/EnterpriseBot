using System.Collections.Generic;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Crafting
{
    public class RecipeCreationParams
    {
        public long Id { get; set; }
        public List<long> IngredientsIds { get; set; }

        public long ResultItemId { get; set; }
        public short ResultItemQuantity { get; set; }

        public int LeadTimeInSeconds { get; set; }
    }
}
