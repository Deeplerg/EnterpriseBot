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
    public class TrunkStorageCategory : StoragesCategoryBase<TrunkStorage,
                                                             long,
                                                             TrunkStorageCreationParams>
    {
        protected const string categoryName = "TrunkStorage";

        public TrunkStorageCategory(IApiClient apiClient) : base(apiClient) { }

        public override async Task<TrunkStorage> Get(long id)
        {
            return await api.Call<TrunkStorage>(RequestInfo(nameof(Get)), IdParameter(id));
        }

        public override async Task<TrunkStorage> Create(TrunkStorageCreationParams pars)
        {
            return await api.Call<TrunkStorage>(RequestInfo(nameof(Create)), pars);
        }


        public async Task<decimal> UpgradeCapacity(long modelId)
        {
            return await UpgradeCapacity(modelId, invokerId: null);
        }

        public async Task<decimal> UpgradeCapacity(long modelId, long invokerPlayerId)
        {
            return await UpgradeCapacity(modelId, invokerId: invokerPlayerId);
        }

        public async Task<bool> HasPermissionToManage(long modelId, long invokerPlayerId)
        {
            return await api.Call<bool>(RequestInfo(nameof(HasPermissionToManage)), new
            {
                modelId = modelId,
                invokerId = invokerPlayerId
            });
        }

        public async Task<bool> ReturnErrorIfDoesNotHavePermissionToManage(long modelId, long invokerPlayerId)
        {
            return await api.Call<bool>(RequestInfo(nameof(ReturnErrorIfDoesNotHavePermissionToManage)), new
            {
                modelId = modelId,
                invokerId = invokerPlayerId
            });
        }


        private async Task<decimal> UpgradeCapacity(long modelId, long? invokerId)
        {
            return await api.Call<decimal>(RequestInfo(nameof(UpgradeCapacity)), new
            {
                modelId = modelId,
                invokerId = invokerId
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
