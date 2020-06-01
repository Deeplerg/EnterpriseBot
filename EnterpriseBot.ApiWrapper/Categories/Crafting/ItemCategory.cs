using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.Common.Crafting;
using EnterpriseBot.ApiWrapper.Models.ModelCreationParams.Crafting;
using EnterpriseBot.ApiWrapper.Models.Other;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Crafting
{
    public class ItemCategory : CraftingCategoryBase<Item>,
                                ICreatableCategory<Item, ItemCreationParams>
    {
        private static readonly string categoryName = "item";

        public ItemCategory(IApiClient api) : base(api) { }

        /// <inheritdoc/>
        public override async Task<Item> Get(object id)
        {
            var pars = new
            {
                id = id
            };

            var result = await api.Call<Item>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Get).ToLower()
            }, pars);

            return result;
        }

        /// <inheritdoc/>
        public async Task<Item> Create(ItemCreationParams pars)
        {
            var result = await api.Call<Item>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Create).ToLower()
            }, pars);

            return result;
        }

        //public void ChangeName(object itemId, string newName) { }

        //public void ChangeCategory(object itemId, CraftingCategory newCategory) { }

        /// <summary>
        /// Returns all items using the specified <paramref name="craftingCategoryName"/> as a filter
        /// </summary>
        /// <param name="craftingCategoryName">Crafting category name as a filter</param>
        /// <returns>All items with the specified <paramref name="craftingCategoryName"/></returns>
        public async Task<List<Item>> GetAllByCategory(string craftingCategoryName)
        {
            var pars = new
            {
                category = craftingCategoryName
            };

            var result = await api.Call<List<Item>>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(GetAllByCategory).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Returns an item which name matches the specified one
        /// </summary>
        /// <param name="name">Item name</param>
        /// <returns>Item which name matches the specified one</returns>
        public async Task<Item> GetByName(string name)
        {
            var pars = new
            {
                name = name
            };

            var result = await api.Call<Item>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(GetByName).ToLower()
            }, pars);

            return result;
        }
    }
}
