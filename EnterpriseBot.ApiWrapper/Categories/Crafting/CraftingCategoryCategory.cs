using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.CreationParams.Crafting;
using EnterpriseBot.ApiWrapper.Models.Enums;
using EnterpriseBot.ApiWrapper.Models.Game.Crafting;
using EnterpriseBot.ApiWrapper.Models.Game.Localization;
using EnterpriseBot.ApiWrapper.Models.Other;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Crafting
{
    public class CraftingCategoryCategory : CraftingCategoryBase<CraftingCategory,
                                                                 long,
                                                                 CraftingCategoryCreationParams>
    {
        protected const string categoryName = "CraftingCategory";

        public CraftingCategoryCategory(IApiClient api) : base(api) { }


        public override async Task<CraftingCategory> Get(long id)
        {
            return await api.Call<CraftingCategory>(RequestInfo(nameof(Get)), IdParameter(id));
        }

        public override async Task<CraftingCategory> Create(CraftingCategoryCreationParams pars)
        {
            return await api.Call<CraftingCategory>(RequestInfo(nameof(Create)), pars);
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

        public async Task<StringLocalization> EditDescription(long modelId, string newDescription, LocalizationLanguage language)
        {
            var pars = new
            {
                modelId = modelId,
                newDescription = newDescription,
                language = language
            };

            return await api.Call<StringLocalization>(RequestInfo(nameof(EditDescription)), pars);
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
