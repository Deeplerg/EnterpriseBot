using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.Common.Crafting;
using EnterpriseBot.ApiWrapper.Models.ModelCreationParams.Crafting;
using EnterpriseBot.ApiWrapper.Models.Other;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Crafting
{
    public class CraftingCategoryCategory : CraftingCategoryBase<CraftingCategory>,
                                            ICreatableCategory<CraftingCategory, CraftingCategoryCreationParams>
    {
        private static readonly string categoryName = "craftingcategory";

        public CraftingCategoryCategory(IApiClient api) : base(api) { }

        /// <inheritdoc/>
        public override async Task<CraftingCategory> Get(object id)
        {
            var pars = new
            {
                id = id
            };

            var result = await api.Call<CraftingCategory>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Get).ToLower()
            }, pars);

            return result;
        }

        /// <inheritdoc/>
        public async Task<CraftingCategory> Create(CraftingCategoryCreationParams pars)
        {
            var result = await api.Call<CraftingCategory>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Create).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Returns all crafting categories
        /// </summary>
        /// <returns>All crafting categories</returns>
        public async Task<List<CraftingCategory>> GetAll()
        {
            var pars = new { };

            var result = await api.Call<List<CraftingCategory>>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(GetAll).ToLower()
            }, pars);

            return result;
        }
    }
}
