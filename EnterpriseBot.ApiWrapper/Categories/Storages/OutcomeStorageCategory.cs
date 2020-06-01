using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.Common.Storages;
using EnterpriseBot.ApiWrapper.Models.Other;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Storages
{
    public class OutcomeStorageCategory : StoragesCategoryBase<OutcomeStorage>
    {
        private static readonly string categoryName = "outcomestorage";

        public OutcomeStorageCategory(IApiClient api) : base(api) { }

        /// <inheritdoc/>
        public override async Task<OutcomeStorage> Get(object id)
        {
            var pars = new
            {
                id = id
            };

            var result = await api.Call<OutcomeStorage>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Get).ToLower()
            }, pars);

            return result;
        }

        ///// <inheritdoc/>
        //public override async Task<OutcomeStorage> Create(OutcomeStorageCreationParams pars)
        //{
        //    var result = await api.Call<OutcomeStorage>(new ApiRequestInfo
        //    {
        //        CategoryAreaName = categoryAreaName,
        //        CategoryName = categoryName,
        //        MethodName = nameof(Create).ToLower()
        //    }, pars);

        //    return result;
        //}

        /// <summary>
        /// Add an item to a storage
        /// </summary>
        /// <param name="outcomeStorageId">Id of outcome storage to which to add the item</param>
        /// <param name="itemId">Id of the item which to add</param>
        /// <param name="quantity">Item quantity</param>
        /// <returns>Storage items</returns>
        public async Task<List<StorageItem>> AddItem(long outcomeStorageId, long itemId, int quantity)
        {
            var pars = new
            {
                outcomeStorageId = outcomeStorageId,
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
        /// Remove an item from a storage
        /// </summary>
        /// <param name="outcomeStorageId">Id of outcome storage from which to remove the item</param>
        /// <param name="itemId">Id of the item which to remove</param>
        /// <param name="quantity">Item quantity</param>
        /// <returns>Storage items</returns>
        public async Task<List<StorageItem>> RemoveItem(long outcomeStorageId, long itemId, int quantity)
        {
            var pars = new
            {
                outcomeStorageId = outcomeStorageId,
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
        /// Moves an item to a trunk storage
        /// </summary>
        /// <param name="outcomeStorageId">Outcome storage id from which to take the item</param>
        /// <param name="trunkStorageId">Trunk storage id to which to add the item</param>
        /// <param name="itemId">Item id</param>
        /// <param name="quantity">Quantity of the item to move</param>
        /// <returns>Trunk storage instance</returns>
        public async Task<TrunkStorage> MoveToTrunkStorage(long outcomeStorageId, long trunkStorageId, long itemId, int quantity)
        {
            var pars = new
            {
                outcomeStorageId = outcomeStorageId,
                trunkStorageId = trunkStorageId,
                itemId = itemId,
                quantity = quantity
            };

            var result = await api.Call<TrunkStorage>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(MoveToTrunkStorage).ToLower()
            }, pars);

            return result;
        }
    }
}
