using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.Common.Crafting;
using EnterpriseBot.ApiWrapper.Models.ModelCreationParams.Crafting;
using EnterpriseBot.ApiWrapper.Models.Other;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Crafting
{
    public class IngredientCategory : CraftingCategoryBase<Ingredient>,
                                      ICreatableCategory<Ingredient, IngredientCreationParams>
    {
        private static readonly string categoryName = "ingredient";

        public IngredientCategory(IApiClient api) : base(api) { }

        /// <inheritdoc/>
        public override async Task<Ingredient> Get(object id)
        {
            var pars = new
            {
                id = id
            };

            var result = await api.Call<Ingredient>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Get).ToLower()
            }, pars);

            return result;
        }

        /// <inheritdoc/>
        public async Task<Ingredient> Create(IngredientCreationParams pars)
        {
            var result = await api.Call<Ingredient>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Create).ToLower()
            }, pars);

            return result;
        }
    }
}
