using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.Common.Storages;
using EnterpriseBot.ApiWrapper.Models.Other;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Storages
{
    public class IncomeStorageCategory : StoragesCategoryBase<IncomeStorage>
    {
        private static readonly string categoryName = "incomestorage";

        public IncomeStorageCategory(IApiClient api) : base(api) { }

        /// <inheritdoc/>
        public override async Task<IncomeStorage> Get(object id)
        {
            var pars = new
            {
                id = id
            };

            var result = await api.Call<IncomeStorage>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Get).ToLower()
            }, pars);

            return result;
        }

        ///// <inheritdoc/>
        //public override async Task<IncomeStorage> Create(IncomeStorageCreationParams pars)
        //{
        //    var result = await api.Call<IncomeStorage>(new ApiRequestInfo
        //    {
        //        CategoryAreaName = categoryAreaName,
        //        CategoryName = categoryName,
        //        MethodName = nameof(Create).ToLower()
        //    }, pars);

        //    return result;
        //}

        /// <summary>
        /// Adds an item to the storage
        /// </summary>
        /// <param name="incomeStorageId">Income storage id to which to add the item</param>
        /// <param name="itemId">Item id which to add</param>
        /// <param name="quantity">Item quantity</param>
        /// <returns>Storage items</returns>
        public async Task<List<StorageItem>> AddItem(long incomeStorageId, long itemId, int quantity)
        {
            var pars = new
            {
                incomeStorageId = incomeStorageId,
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
        /// Remove an item from the storage
        /// </summary>
        /// <param name="incomeStorageId">Income storage id from which to remove the item</param>
        /// <param name="itemId">Item id which to remove</param>
        /// <returns>Storage items</returns>
        public async Task<List<StorageItem>> RemoveItem(long incomeStorageId, long itemId, int quantity)
        {
            var pars = new
            {
                incomeStorageId = incomeStorageId,
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
        /// Moves an item to a worker storage
        /// </summary>
        /// <param name="incomeStorageId">Income storage id from which to take the item</param>
        /// <param name="workerStorageId">Worker storage id to which to add the item</param>
        /// <param name="itemId">Item id</param>
        /// <param name="quantity">Quantity of the item to move</param>
        /// <returns>Income storage instance</returns>
        public async Task<IncomeStorage> MoveToWorkerStorage(long incomeStorageId, long workerStorageId, long itemId, int quantity)
        {
            var pars = new
            {
                incomeStorageId = incomeStorageId,
                workerStorageId = workerStorageId,
                itemId = itemId,
                quantity = quantity
            };

            var result = await api.Call<IncomeStorage>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(MoveToWorkerStorage).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Moves an item to showcase storage
        /// <param name="incomeStorageId">Income storage id from which to take the item</param>
        /// <param name="showcaseStorageId">Showcase storage id to which to add the item</param>
        /// <param name="itemId">Item id</param>
        /// <param name="quantity">Quantity of the item to move</param>
        /// </summary>
        /// <returns>Income storage instance</returns>
        public async Task<IncomeStorage> MoveToShowcaseStorage(long incomeStorageId, long showcaseStorageId, long itemId, int quantity, decimal price)
        {
            var pars = new
            {
                incomeStorageId = incomeStorageId,
                showcaseStorageId = showcaseStorageId,
                itemId = itemId,
                quantity = quantity,
                price = price
            };

            var result = await api.Call<IncomeStorage>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(MoveToShowcaseStorage).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Unloads the truck to the storage
        /// </summary>
        /// <param name="incomeStorageId">Id of income storage to which to add items from the truck</param>
        /// <param name="truckId">Id of the truck from which to unload the items</param>
        /// <param name="contractId">Id of the contract for delivery</param>
        public async Task UnloadTruck(long incomeStorageId, long truckId, long contractId)
        {
            var pars = new
            {
                incomeStorageId = incomeStorageId,
                truckId = truckId,
                contractId = contractId
            };

            await api.Call(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(UnloadTruck)
            }, pars);
        }
    }
}
