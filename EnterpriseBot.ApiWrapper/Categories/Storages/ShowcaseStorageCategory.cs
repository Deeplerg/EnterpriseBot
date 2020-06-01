using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.Common.Storages;
using EnterpriseBot.ApiWrapper.Models.Other;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Storages
{
    public class ShowcaseStorageCategory : StoragesCategoryBase<ShowcaseStorage>
    {
        private static readonly string categoryName = "showcasestorage";

        public ShowcaseStorageCategory(IApiClient api) : base(api) { }

        /// <inheritdoc/>
        public override async Task<ShowcaseStorage> Get(object id)
        {
            var pars = new
            {
                id = id
            };

            var result = await api.Call<ShowcaseStorage>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Get).ToLower()
            }, pars);

            return result;
        }

        ///// <inheritdoc/>
        //public override async Task<ShowcaseStorage> Create(ShowcaseStorageCreationParams pars)
        //{
        //    var result = await api.Call<ShowcaseStorage>(new ApiRequestInfo
        //    {
        //        CategoryAreaName = categoryAreaName,
        //        CategoryName = categoryName,
        //        MethodName = nameof(Create).ToLower()
        //    }, pars);

        //    return result;
        //}

        /// <summary>
        /// Adds an item to a storage
        /// </summary>
        /// <param name="showcaseStorageId">Id of showcase storage to which to add the item</param>
        /// <param name="itemId">Item id</param>
        /// <param name="quantity">Item quantity</param>
        /// <param name="price">Item price (for each one)</param>
        /// <returns>Storage items</returns>
        public async Task<List<StorageItemWithPrice>> AddItem(long showcaseStorageId, long itemId, int quantity, decimal price)
        {
            var pars = new
            {
                showcaseStorageId = showcaseStorageId,
                itemId = itemId,
                quantity = quantity,
                price = price
            };

            var result = await api.Call<List<StorageItemWithPrice>>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(AddItem).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Removes an item from a storage
        /// </summary>
        /// <param name="showcaseStorageId">Id of showcase storage from which to remove an item</param>
        /// <param name="storageItemWithPriceId">Id of <see cref="StorageItemWithPrice"/> which to remove</param>
        /// <returns>Storage items</returns>
        public async Task<List<StorageItemWithPrice>> RemoveItem(long showcaseStorageId, long storageItemWithPriceId, int quantity)
        {
            var pars = new
            {
                showcaseStorageId = showcaseStorageId,
                storageItemWithPriceId = storageItemWithPriceId,
                quantity = quantity
            };

            var result = await api.Call<List<StorageItemWithPrice>>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(RemoveItem).ToLower()
            }, pars);

            return result;
        }
    }
}
