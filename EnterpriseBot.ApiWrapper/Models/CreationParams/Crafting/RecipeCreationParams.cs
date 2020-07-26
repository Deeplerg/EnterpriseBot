using EnterpriseBot.ApiWrapper.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
