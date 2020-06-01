using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.Common.Crafting;
using EnterpriseBot.ApiWrapper.Models.ModelCreationParams.Crafting;
using EnterpriseBot.ApiWrapper.Models.Other;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Crafting
{
    public class RecipeCategory : CraftingCategoryBase<Recipe>,
                                  ICreatableCategory<Recipe, RecipeCreationParams>
    {
        private const string categoryName = "recipe";

        public RecipeCategory(IApiClient api) : base(api) { }

        /// <inheritdoc/>
        public override async Task<Recipe> Get(object id)
        {
            var pars = new
            {
                id = id
            };

            var result = await api.Call<Recipe>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Get).ToLower()
            }, pars);

            return result;
        }

        /// <inheritdoc/>
        public async Task<Recipe> Create(RecipeCreationParams pars)
        {
            var result = await api.Call<Recipe>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Create).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Returns recipes where result item matches the specified one
        /// </summary>
        /// <param name="itemId">Item id</param>
        /// <returns>Recipes for the specified item</returns>
        public async Task<List<Recipe>> GetRecipesForItem(long itemId)
        {
            var pars = new
            {
                itemId = itemId
            };

            var result = await api.Call<List<Recipe>>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(GetRecipesForItem).ToLower()
            }, pars);

            return result;
        }
    }
}
