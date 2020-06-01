using System.Collections.Generic;

namespace EnterpriseBot.Api.Models.Common.Crafting
{
    public class Recipe
    {
        public long Id { get; set; }
        public virtual List<Ingredient> Ingredients { get; set; }

        public long ResultItemId { get; set; }
        public virtual Item ResultItem { get; set; }

        public int LeadTimeInSeconds { get; set; } //how much time in seconds it takes to produce it

        public short ResultItemQuantity { get; set; }
    }
}
