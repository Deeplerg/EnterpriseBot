using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.Common.Storages;
using EnterpriseBot.ApiWrapper.Models.Other;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Storages
{
    public class WorkerStorageCategory : StoragesCategoryBase<WorkerStorage>
    {
        private static readonly string categoryName = "workerstorage";

        public WorkerStorageCategory(IApiClient api) : base(api) { }

        /// <inheritdoc/>
        public override async Task<WorkerStorage> Get(object id)
        {
            var pars = new
            {
                id = id
            };

            var result = await api.Call<WorkerStorage>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Get).ToLower()
            }, pars);

            return result;
        }

        ///// <inheritdoc/>
        //public override async Task<WorkerStorage> Create(WorkerStorageCreationParams pars)
        //{
        //    var result = await api.Call<WorkerStorage>(new ApiRequestInfo
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
        /// <param name="workerStorageId">Id of worker storage to which to add the item</param>
        /// <param name="itemId">Id of the item which to add</param>
        /// <param name="quantity">Item quantity</param>
        /// <returns>Storage items</returns>
        public async Task<List<StorageItem>> AddItem(long workerStorageId, long itemId, int quantity)
        {
            var pars = new
            {
                workerStorageId = workerStorageId,
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
        /// <param name="workerStorageId">Id of worker storage from which to remove an item</param>
        /// <param name="itemId">Id of the item which to remove</param>
        /// <returns>Storage items</returns>
        public async Task<List<StorageItem>> RemoveItem(long workerStorageId, long itemId, int quantity)
        {
            var pars = new
            {
                workerStorageId = workerStorageId,
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
        /// Moves an item to an outcome storage
        /// </summary>
        /// <param name="workerStorageId">Id of worker storage from which to take the item</param>
        /// <param name="outcomeStorageId">Id of outcome storage to which to add </param>
        /// <param name="itemId">Id of the item which to move</param>
        /// <param name="quantity">Item quantity</param>
        /// <returns>Worker storage instance</returns>
        public async Task<WorkerStorage> MoveToOutcomeStorage(long workerStorageId, long outcomeStorageId, long itemId, int quantity)
        {
            var pars = new
            {
                workerStorageId = workerStorageId,
                outcomeStorageId = outcomeStorageId,
                itemId = itemId,
                quantity = quantity
            };

            var result = await api.Call<WorkerStorage>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(MoveToOutcomeStorage).ToLower()
            }, pars);

            return result;
        }
    }
}
