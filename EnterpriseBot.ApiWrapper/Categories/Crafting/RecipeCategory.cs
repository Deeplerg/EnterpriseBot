using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.CreationParams.Crafting;
using EnterpriseBot.ApiWrapper.Models.Enums;
using EnterpriseBot.ApiWrapper.Models.Game.Crafting;
using EnterpriseBot.ApiWrapper.Models.Other;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Crafting
{
    public class RecipeCategory : CraftingCategoryBase<Recipe,
                                                       long,
                                                       RecipeCreationParams>
    {
        protected const string categoryName = "Recipe";

        public RecipeCategory(IApiClient api) : base(api) { }

        public override async Task<Recipe> Get(long id)
        {
            return await api.Call<Recipe>(RequestInfo(nameof(Get)), IdParameter(id));
        }

        public override async Task<Recipe> Create(RecipeCreationParams pars)
        {
            return await api.Call<Recipe>(RequestInfo(nameof(Create)), pars);
        }


        public async Task RemoveIngredient(long modelId, long ingredientId)
        {
            var pars = new
            {
                modelId = modelId,
                ingredientId = ingredientId
            };

            await api.Call(RequestInfo(nameof(RemoveIngredient)), pars);
        }

        public async Task<int> SetLeadTimeInSeconds(long modelId, int newLeadTimeInSeconds)
        {
            var pars = new
            {
                modelId = modelId,
                newLeadTimeInSeconds = newLeadTimeInSeconds
            };

            return await api.Call<int>(RequestInfo(nameof(SetLeadTimeInSeconds)), pars);
        }

        public async Task<int> SetResultItemQuantity(long modelId, int newResultItemQuantity)
        {
            var pars = new
            {
                modelId = modelId,
                newResultItemQuantity = newResultItemQuantity
            };

            return await api.Call<int>(RequestInfo(nameof(SetResultItemQuantity)), pars);
        }

        public async Task<RecipeCanBeDoneBy> SetCanBeDoneBy(long modelId, RecipeCanBeDoneBy newCanBeDoneBy)
        {
            var pars = new
            {
                modelId = modelId,
                newCanBeDoneBy = newCanBeDoneBy
            };

            return await api.Call<RecipeCanBeDoneBy>(RequestInfo(nameof(SetCanBeDoneBy)), pars);
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
