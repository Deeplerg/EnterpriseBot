using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.CreationParams.Storages;
using EnterpriseBot.ApiWrapper.Models.Game.Storages;
using EnterpriseBot.ApiWrapper.Models.Other;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Storages
{
    public class ShowcaseStorageCategory : StoragesCategoryBase<ShowcaseStorage,
                                                                long,
                                                                ShowcaseStorageCreationParams>
    {
        protected const string categoryName = "ShowcaseStorage";

        public ShowcaseStorageCategory(IApiClient apiClient) : base(apiClient) { }

        public override async Task<ShowcaseStorage> Get(long id)
        {
            return await api.Call<ShowcaseStorage>(RequestInfo(nameof(Get)), IdParameter(id));
        }

        public override async Task<ShowcaseStorage> Create(ShowcaseStorageCreationParams pars)
        {
            return await api.Call<ShowcaseStorage>(RequestInfo(nameof(Create)), pars);
        }


        public async Task<ItemPrice> AddPrice(long modelId, long itemId, decimal price, long invokerPlayerId)
        {
            return await api.Call<ItemPrice>(RequestInfo(nameof(AddPrice)), new
            {
                modelId = modelId,
                itemId = itemId,
                price = price,
                invokerId = invokerPlayerId
            });
        }

        public async Task<ItemPrice> SetPrice(long modelId, long itemId, decimal price, long invokerPlayerId)
        {
            return await api.Call<ItemPrice>(RequestInfo(nameof(SetPrice)), new
            {
                modelId = modelId,
                itemId = itemId,
                price = price,
                invokerId = invokerPlayerId
            });
        }

        public async Task<ItemPrice> GetPrice(long modelId, long itemId)
        {
            return await api.Call<ItemPrice>(RequestInfo(nameof(GetPrice)), new
            {
                modelId = modelId,
                itemId = itemId
            });
        }

        public async Task<bool> IsPriceDefinedForItem(long modelId, long itemId)
        {
            return await api.Call<bool>(RequestInfo(nameof(IsPriceDefinedForItem)), new
            {
                modelId = modelId,
                itemId = itemId
            });
        }

        public async Task<int> BuyItem(long modelId, long itemId, int quantity, long buyerId)
        {
            return await api.Call<int>(RequestInfo(nameof(BuyItem)), new
            {
                modelId = modelId,
                itemId = itemId,
                quantity = quantity,
                buyerId = buyerId
            });
        }

        public async Task<decimal> UpgradeCapacity(long modelId)
        {
            return await UpgradeCapacity(modelId, invokerId: null);
        }

        public async Task<decimal> UpgradeCapacity(long modelId, long invokerPlayerId)
        {
            return await UpgradeCapacity(modelId, invokerId: invokerPlayerId);
        }

        private async Task<decimal> UpgradeCapacity(long modelId, long? invokerId)
        {
            return await api.Call<decimal>(RequestInfo(nameof(UpgradeCapacity)), new
            {
                modelId = modelId,
                invokerId = invokerId
            });
        }

        public async Task<bool> HasPermissionToManage(long modelId, long invokerPlayerId)
        {
            return await api.Call<bool>(RequestInfo(nameof(HasPermissionToManage)), new
            {
                modelId = modelId,
                invokerId = invokerPlayerId
            });
        }

        public async Task<bool> HasPermissionToManagePrices(long modelId, long invokerPlayerId)
        {
            return await api.Call<bool>(RequestInfo(nameof(HasPermissionToManagePrices)), new
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

        public async Task<bool> ReturnErrorIfDoesNotHavePermissionToManagePrices(long modelId, long invokerPlayerId)
        {
            return await api.Call<bool>(RequestInfo(nameof(ReturnErrorIfDoesNotHavePermissionToManagePrices)), new
            {
                modelId = modelId,
                invokerId = invokerPlayerId
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
