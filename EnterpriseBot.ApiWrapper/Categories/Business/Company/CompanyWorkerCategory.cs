using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.CreationParams.Business.Company;
using EnterpriseBot.ApiWrapper.Models.Game.Business.Company;
using EnterpriseBot.ApiWrapper.Models.Other;
using System;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Business.Company
{
    public class CompanyWorkerCategory : CompanySubCategoryBase<CompanyWorker,
                                                                long,
                                                                CompanyWorkerCreationParams>
    {
        protected const string categoryName = "CompanyWorker";

        public CompanyWorkerCategory(IApiClient apiClient) : base(apiClient) { }

        public override async Task<CompanyWorker> Get(long id)
        {
            return await api.Call<CompanyWorker>(RequestInfo(nameof(Get)), IdParameter(id));
        }

        public override async Task<CompanyWorker> Create(CompanyWorkerCreationParams pars)
        {
            return await api.Call<CompanyWorker>(RequestInfo(nameof(Create)), pars);
        }


        public async Task StartWorking(long modelId)
        {
            await api.Call(RequestInfo(nameof(StartWorking)), new
            {
                modelId = modelId
            });
        }

        [Obsolete("Should not be used in a normal flow")]
        public async Task ProduceItem(long modelId)
        {
            await api.Call(RequestInfo(nameof(ProduceItem)), new
            {
                modelId = modelId
            });
        }

        public async Task StopWorking(long modelId)
        {
            await api.Call(RequestInfo(nameof(StopWorking)), new
            {
                modelId = modelId
            });
        }

        public async Task SetWorkingStorage(long modelId, long newWorkingCompanyStorageId)
        {
            await api.Call(RequestInfo(nameof(SetWorkingStorage)), new
            {
                modelId = modelId,
                companyStorageId = newWorkingCompanyStorageId
            });
        }

        public async Task SetRecipe(long modelId, long newRecipeId)
        {
            await api.Call(RequestInfo(nameof(SetRecipe)), new
            {
                modelId = modelId,
                recipeId = newRecipeId
            });
        }

        public async Task<decimal> UpgradeSpeedMultiplier(long modelId, decimal step)
        {
            return await api.Call<decimal>(RequestInfo(nameof(UpgradeSpeedMultiplier)), new
            {
                modelId = modelId,
                step = step
            });
        }



        private ApiRequestInfo RequestInfo(string methodName)
        {
            return new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategorySubAreaName = categorySubAreaName,
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
