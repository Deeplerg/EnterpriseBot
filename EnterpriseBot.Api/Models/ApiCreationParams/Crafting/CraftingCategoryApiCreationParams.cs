using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.ApiCreationParams.Crafting
{
    public class CraftingCategoryApiCreationParams
    {
        public long NameLocalizedStringId { get; set; }
        public long DescriptionLocalizedStringId { get; set; }

        /// <summary>
        /// Optional
        /// </summary>
        public List<long> CraftingSubCategoriesIds { get; set; }
    }
}
