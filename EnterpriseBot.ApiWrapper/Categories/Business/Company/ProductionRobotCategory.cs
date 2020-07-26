using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.CreationParams.Business.Company;
using EnterpriseBot.ApiWrapper.Models.Game.Business.Company;
using EnterpriseBot.ApiWrapper.Models.Other;
using System;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Business.Company
{
    public class ProductionRobotCategory : CompanySubCategoryBase<ProductionRobot,
                                                                  long,
                                                                  ProductionRobotCreationParams>
    {
        protected const string categoryName = "ProductionRobot";

        public ProductionRobotCategory(IApiClient apiClient) : base(apiClient) { }

        public override async Task<ProductionRobot> Get(long id)
        {
            return await api.Call<ProductionRobot>(RequestInfo(nameof(Get)), IdParameter(id));
        }

        public override async Task<ProductionRobot> Create(ProductionRobotCreationParams pars)
        {
            return await api.Call<ProductionRobot>(RequestInfo(nameof(Create)), pars);
        }


        public async Task<ProductionRobot> BuyAndCreate(long companyId, ProductionRobotCreationParams pars)
        {
            await Buy(companyId);
            return await Create(pars);
        }


        public async Task StartWorking(long modelId, long invokerPlayerId)
        {
            await api.Call(RequestInfo(nameof(StartWorking)), new
            {
                modelId = modelId,
                invokerId = invokerPlayerId
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

        public async Task StopWorking(long modelId, long invokerPlayerId)
        {
            await api.Call(RequestInfo(nameof(StopWorking)), new
            {
                modelId = modelId,
                invokerId = invokerPlayerId
            });
        }

        public async Task SetWorkingStorage(long modelId, long newWorkingCompanyStorageId, long invokerPlayerId)
        {
            await api.Call(RequestInfo(nameof(SetWorkingStorage)), new
            {
                modelId = modelId,
                companyStorageId = newWorkingCompanyStorageId,
                invokerId = invokerPlayerId
            });
        }

        public async Task SetRecipe(long modelId, long newRecipeId, long invokerPlayerId)
        {
            await api.Call(RequestInfo(nameof(SetRecipe)), new
            {
                modelId = modelId,
                recipeId = newRecipeId,
                invokerId = invokerPlayerId
            });
        }

        public async Task Upgrade(long modelId, decimal step, long invokerPlayerId)
        {
            await api.Call(RequestInfo(nameof(Upgrade)), new
            {
                modelId = modelId,
                step = step,
                invokerId = invokerPlayerId
            });
        }



        private async Task Buy(long companyId)
        {
            await api.Call(RequestInfo(nameof(Buy)), new
            {
                companyId = companyId
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
