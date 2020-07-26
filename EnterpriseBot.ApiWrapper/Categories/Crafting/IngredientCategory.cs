using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.CreationParams.Crafting;
using EnterpriseBot.ApiWrapper.Models.Game.Crafting;
using EnterpriseBot.ApiWrapper.Models.Other;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Crafting
{
    public class IngredientCategory : CraftingCategoryBase<Ingredient,
                                                           long,
                                                           IngredientCreationParams>
    {
        protected const string categoryName = "Ingredient";

        public IngredientCategory(IApiClient api) : base(api) { }

        public override async Task<Ingredient> Get(long id)
        {
            return await api.Call<Ingredient>(RequestInfo(nameof(Get)), IdParameter(id));
        }

        public override async Task<Ingredient> Create(IngredientCreationParams pars)
        {
            return await api.Call<Ingredient>(RequestInfo(nameof(Create)), pars);
        }

        public async Task<int> SetQuantity(long modelId, int newQuantity)
        {
            var pars = new
            {
                modelId = modelId,
                newQuantity = newQuantity
            };

            return await api.Call<int>(RequestInfo(nameof(SetQuantity)), pars);
        }


        private ApiRequestInfo RequestInfo(string methodName)
        {
            return new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = methodName
            };
        }

        private object IdParameter(long id)
        {
            return new
            {
                id = id
            };
        }
    }
}
