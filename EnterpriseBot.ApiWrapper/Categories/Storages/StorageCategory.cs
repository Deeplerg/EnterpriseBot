using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.CreationParams.Storages;
using EnterpriseBot.ApiWrapper.Models.Game.Storages;
using EnterpriseBot.ApiWrapper.Models.Other;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Storages
{
    public class StorageCategory : StoragesCategoryBase<Storage,
                                                        long,
                                                        StorageCreationParams>
    {
        protected const string categoryName = "Storage";

        public StorageCategory(IApiClient apiClient) : base(apiClient) { }

        public override async Task<Storage> Get(long id)
        {
            return await api.Call<Storage>(RequestInfo(nameof(Get)), IdParameter(id));
        }

        public override async Task<Storage> Create(StorageCreationParams pars)
        {
            return await api.Call<Storage>(RequestInfo(nameof(Create)), pars);
        }


        public async Task<int> AddItem(long modelId, long itemId, int quantity)
        {
            return await api.Call<int>(RequestInfo(nameof(AddItem)), new
            {
                modelId = modelId,
                itemId = itemId,
                quantity = quantity
            });
        }

        public async Task<int> RemoveItem(long modelId, long itemId, int quantity)
        {
            return await api.Call<int>(RequestInfo(nameof(RemoveItem)), new
            {
                modelId = modelId,
                itemId = itemId,
                quantity = quantity
            });
        }

        public async Task<int> ContainsItem(long modelId, long itemId, int quantity)
        {
            return await api.Call<int>(RequestInfo(nameof(ContainsItem)), new
            {
                modelId = modelId,
                itemId = itemId,
                quantity = quantity
            });
        }

        public async Task<int> TransferItemToStorage(long modelId, long receivingStorageId, long itemId, int quantity)
        {
            return await api.Call<int>(RequestInfo(nameof(TransferItemToStorage)), new
            {
                modelId = modelId,
                receivingStorageId = receivingStorageId,
                itemId = itemId,
                quantity = quantity
            });
        }

        public async Task<int> TransferEverythingToStorage(long modelId, long receivingStorageId)
        {
            return await TransferEverythingToStorage(modelId, receivingStorageId, itemTypeToTransferId: null);
        }

        public async Task<int> TransferEverythingToStorage(long modelId, long receivingStorageId, long itemToTransferId)
        {
            return await TransferEverythingToStorage(modelId, receivingStorageId, itemTypeToTransferId: itemToTransferId);
        }

        public async Task<StorageItem> GetItem(long modelId, long itemId)
        {
            return await api.Call<StorageItem>(RequestInfo(nameof(GetItem)), new
            {
                modelId = modelId,
                itemId = itemId
            });
        }

        public async Task<decimal> AddCapacity(long modelId, decimal amount)
        {
            return await api.Call<decimal>(RequestInfo(nameof(AddCapacity)), new
            {
                modelId = modelId,
                amount = amount
            });
        }



        private async Task<int> TransferEverythingToStorage(long modelId, long receivingStorageId, long? itemTypeToTransferId)
        {
            return await api.Call<int>(RequestInfo(nameof(TransferEverythingToStorage)), new
            {
                modelId = modelId,
                receivingStorageId = receivingStorageId,
                itemTypeToTransferId = itemTypeToTransferId
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
