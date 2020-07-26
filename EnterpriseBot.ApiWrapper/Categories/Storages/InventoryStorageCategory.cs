using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.Game.Storages;
using EnterpriseBot.ApiWrapper.Models.Other;
using EnterpriseBot.ApiWrapper.Models.CreationParams.Storages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Storages
{
    public class InventoryStorageCategory : StoragesCategoryBase<InventoryStorage,
                                                          long,
                                                          InventoryStorageCreationParams>
    {
        protected const string categoryName = "InventoryStorage";

        public InventoryStorageCategory(IApiClient apiClient) : base(apiClient) { }

        public override async Task<InventoryStorage> Get(long id)
        {
            return await api.Call<InventoryStorage>(RequestInfo(nameof(Get)), IdParameter(id));
        }

        public override async Task<InventoryStorage> Create(InventoryStorageCreationParams pars)
        {
            return await api.Call<InventoryStorage>(RequestInfo(nameof(Create)), pars);
        }


        public async Task<decimal> UpgradeCapacity(long modelId)
        {
            return await api.Call<decimal>(RequestInfo(nameof(UpgradeCapacity)), new
            {
                modelId = modelId
            });
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
