using EnterpriseBot.Api.Models.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.ApiCreationParams.Crafting
{
    public class RecipeApiCreationParams
    {
        public List<long> IngredientsIds { get; set; }

        public long ResultItemId { get; set; }
        public short ResultItemQuantity { get; set; }

        public int LeadTimeInSeconds { get; set; }

        public RecipeCanBeDoneBy CanBeDoneBy { get; set; }
    }
}
