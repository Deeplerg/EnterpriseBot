using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.CreationParams.Storages;
using EnterpriseBot.ApiWrapper.Models.Game.Storages;
using EnterpriseBot.ApiWrapper.Models.Other;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Storages
{
    public class StorageItemCategory : StoragesCategoryBase<StorageItem,
                                                            long,
                                                            StorageItemCreationParams>
    {
        protected const string categoryName = "StorageItem";

        public StorageItemCategory(IApiClient apiClient) : base(apiClient) { }

        public override async Task<StorageItem> Get(long id)
        {
            return await api.Call<StorageItem>(RequestInfo(nameof(Get)), IdParameter(id));
        }

        public override async Task<StorageItem> Create(StorageItemCreationParams pars)
        {
            return await api.Call<StorageItem>(RequestInfo(nameof(Create)), pars);
        }


        public async Task<int> AddQuantity(long modelId, int amount)
        {
            return await api.Call<int>(RequestInfo(nameof(AddQuantity)), new
            {
                modelId = modelId,
                amount = amount
            });
        }

        public async Task<int> ReduceQuantity(long modelId, int amount)
        {
            return await api.Call<int>(RequestInfo(nameof(ReduceQuantity)), new
            {
                modelId = modelId,
                amount = amount
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
