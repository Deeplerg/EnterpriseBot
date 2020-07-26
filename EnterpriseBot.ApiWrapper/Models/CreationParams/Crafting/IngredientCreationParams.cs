using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Models.CreationParams.Crafting
{
    public class IngredientCreationParams
    {
        public long ItemId { get; set; }
        public int Quantity { get; set; }

        public long RecipeId { get; set; }
    }
}
