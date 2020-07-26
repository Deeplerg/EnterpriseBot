using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.CreationParams.Storages;
using EnterpriseBot.ApiWrapper.Models.Game.Storages;
using EnterpriseBot.ApiWrapper.Models.Other;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Storages
{
    public class CompanyStorageCategory : StoragesCategoryBase<CompanyStorage,
                                                               long,
                                                               CompanyStorageCreationParams>
    {
        protected const string categoryName = "CompanyStorage";

        public CompanyStorageCategory(IApiClient apiClient) : base(apiClient) { }

        public override async Task<CompanyStorage> Get(long id)
        {
            return await api.Call<CompanyStorage>(RequestInfo(nameof(Get)), IdParameter(id));
        }

        public override async Task<CompanyStorage> Create(CompanyStorageCreationParams pars)
        {
            return await api.Call<CompanyStorage>(RequestInfo(nameof(Create)), pars);
        }


        public async Task<CompanyStorage> BuyAndCreate(long companyId, CompanyStorageCreationParams pars, long invokerPlayerId)
        {
            return await api.Call<CompanyStorage>(RequestInfo(nameof(BuyAndCreate)), new
            {
                companyId = companyId,
                companyStorageCreationParams = pars,
                invokerId = invokerPlayerId
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

        public async Task<bool> HasPermissionToManage(long modelId, long invokerId)
        {
            return await api.Call<bool>(RequestInfo(nameof(HasPermissionToManage)), new
            {
                modelId = modelId,
                invokerId = invokerId
            });
        }

        public async Task ReturnErrorIfDoesNotHavePermissionToManage(long modelId, long invokerId)
        {
            await api.Call(RequestInfo(nameof(ReturnErrorIfDoesNotHavePermissionToManage)), new
            {
                modelId = modelId,
                invokerId = invokerId
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
