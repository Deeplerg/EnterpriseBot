using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.CreationParams.Crafting;
using EnterpriseBot.ApiWrapper.Models.Enums;
using EnterpriseBot.ApiWrapper.Models.Game.Crafting;
using EnterpriseBot.ApiWrapper.Models.Game.Localization;
using EnterpriseBot.ApiWrapper.Models.Other;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Crafting
{
    public class ItemCategory : CraftingCategoryBase<Item,
                                                     long,
                                                     ItemCreationParams>
    {
        protected const string categoryName = "Item";

        public ItemCategory(IApiClient api) : base(api) { }

        public override async Task<Item> Get(long id)
        {
            return await api.Call<Item>(RequestInfo(nameof(Get)), IdParameter(id));
        }

        public override async Task<Item> Create(ItemCreationParams pars)
        {
            return await api.Call<Item>(RequestInfo(nameof(Create)), pars);
        }

        public async Task<StringLocalization> EditName(long modelId, string newName, LocalizationLanguage language)
        {
            var pars = new
            {
                modelId = modelId,
                newName = newName,
                language = language
            };

            return await api.Call<StringLocalization>(RequestInfo(nameof(EditName)), pars);
        }

        public async Task<decimal> SetSpace(long modelId, decimal newSpace)
        {
            var pars = new
            {
                modelId = modelId,
                newSpace = newSpace
            };

            return await api.Call<decimal>(RequestInfo(nameof(SetSpace)), pars);
        }

        public async Task<CraftingSubCategory> SetCategory(long modelId, long newCraftingSubCategoryId)
        {
            var pars = new
            {
                modelId = modelId,
                newCraftingSubCategoryId = newCraftingSubCategoryId
            };

            return await api.Call<CraftingSubCategory>(RequestInfo(nameof(SetCategory)), pars);
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
