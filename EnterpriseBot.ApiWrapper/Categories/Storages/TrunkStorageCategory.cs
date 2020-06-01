using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.Common.Storages;
using EnterpriseBot.ApiWrapper.Models.Other;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Storages
{
    public class TrunkStorageCategory : StoragesCategoryBase<TrunkStorage>
    {
        private static readonly string categoryName = "trunkstorage";

        public TrunkStorageCategory(IApiClient api) : base(api) { }

        /// <inheritdoc/>
        public override async Task<TrunkStorage> Get(object id)
        {
            var pars = new
            {
                id = id
            };

            var result = await api.Call<TrunkStorage>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Get).ToLower()
            }, pars);

            return result;
        }

        ///// <inheritdoc/>
        //public override async Task<TrunkStorage> Create(TrunkStorageCreationParams pars)
        //{
        //    var result = await api.Call<TrunkStorage>(new ApiRequestInfo
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
        /// <param name="trunkStorageId">Id of trunk storage from which to take the item</param>
        /// <param name="itemId">Id of the item which to add</param>
        /// <param name="quantity">Item quantity</param>
        /// <returns>Storage items</returns>
        public async Task<List<StorageItem>> AddItem(long trunkStorageId, long itemId, int quantity)
        {
            var pars = new
            {
                trunkStorageId = trunkStorageId,
                itemId = itemId,
                quantity = quantity
            };

            var result = await api.Call<List<StorageItem>>(new ApiRequestInfo
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
        /// <param name="trunkStorageId">Id of trunk storage from which to remove the item</param>
        /// <param name="itemId">Id of the item which to remove</param>
        /// <param name="quantity">Item quantity</param>
        /// <returns>Storage items</returns>
        public async Task<List<StorageItem>> RemoveItem(long trunkStorageId, long itemId, int quantity)
        {
            var pars = new
            {
                trunkStorageId = trunkStorageId,
                itemId = itemId,
                quantity = quantity
            };

            var result = await api.Call<List<StorageItem>>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(RemoveItem).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Moves an item to an income storage
        /// </summary>
        /// <param name="trunkStorageId">Id of trunk storage from which to take the item</param>
        /// <param name="incomeStorageId">Id of income storage from to which to add the item</param>
        /// <param name="itemId">Id of the item which to move</param>
        /// <param name="quantity">Item quantity</param>
        /// <returns>Trunk storage</returns>
        public async Task<TrunkStorage> MoveToIncomeStorage(long trunkStorageId, long incomeStorageId, long itemId, int quantity)
        {
            var pars = new
            {
                trunkStorageId = trunkStorageId,
                incomeStorageId = incomeStorageId,
                itemId = itemId,
                quantity = quantity
            };

            var result = await api.Call<TrunkStorage>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(MoveToIncomeStorage).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Clears the storage
        /// </summary>
        /// <param name="trunkStorageId">Id of the trunk storage which to clear</param>
        /// <returns></returns>
        public async Task Clear(long trunkStorageId)
        {
            var pars = new
            {
                trunkStorageId = trunkStorageId
            };

            await api.Call(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Clear).ToLower()
            }, pars);
        }
    }
}
