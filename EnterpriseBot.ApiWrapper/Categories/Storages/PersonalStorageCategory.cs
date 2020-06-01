using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.Common.Storages;
using EnterpriseBot.ApiWrapper.Models.Other;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Storages
{
    public class PersonalStorageCategory : StoragesCategoryBase<PersonalStorage>
    {
        private static readonly string categoryName = "personalstorage";

        public PersonalStorageCategory(IApiClient api) : base(api) { }

        /// <inheritdoc/>
        public override async Task<PersonalStorage> Get(object id)
        {
            var pars = new
            {
                id = id
            };

            var result = await api.Call<PersonalStorage>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Get).ToLower()
            }, pars);

            return result;
        }

        ///// <inheritdoc/>
        //public override async Task<PersonalStorage> Create(PersonalStorageCreationParams pars)
        //{
        //    var result = await api.Call<PersonalStorage>(new ApiRequestInfo
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
        /// <param name="personalStorageId">Id of personal storage to which to add the item</param>
        /// <param name="itemId">Item id</param>
        /// <param name="quantity">Item quantity</param>
        /// <returns>Storage items</returns>
        public async Task<List<StorageItem>> AddItem(long personalStorageId, long itemId, int quantity)
        {
            var pars = new
            {
                personalStorage_id = personalStorageId,
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
        /// Removes an item from the storage
        /// </summary>
        /// <param name="personalStorageId">Id of personal storage from which to remove the item</param>
        /// <param name="itemId">Item id</param>
        /// <param name="quantity">Item quantity</param>
        /// <returns>Storage items</returns>
        public async Task<List<StorageItem>> RemoveItem(long personalStorageId, long itemId, int quantity)
        {
            var pars = new
            {
                personalStorageId = personalStorageId,
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
        /// Removes all items from a storage
        /// </summary>
        /// <param name="personalStorageId">Id of personal storage which to clear</param>
        public async Task Clear(long personalStorageId)
        {
            var pars = new
            {
                personalStorageId = personalStorageId
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
