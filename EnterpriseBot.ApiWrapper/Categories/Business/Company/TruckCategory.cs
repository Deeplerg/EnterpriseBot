using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.CreationParams.Business.Company;
using EnterpriseBot.ApiWrapper.Models.Game.Business.Company;
using EnterpriseBot.ApiWrapper.Models.Other;
using System;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Business.Company
{
    public class TruckCategory : CompanySubCategoryBase<Truck,
                                                        long,
                                                        TruckCreationParams>
    {
        protected const string categoryName = "Truck";

        public TruckCategory(IApiClient apiClient) : base(apiClient) { }

        public override async Task<Truck> Get(long id)
        {
            return await api.Call<Truck>(RequestInfo(nameof(Get)), IdParameter(id));
        }

        public override async Task<Truck> Create(TruckCreationParams pars)
        {
            return await api.Call<Truck>(RequestInfo(nameof(Create)), pars);
        }


        public async Task Send(long modelId, long contractId, long invokerPlayerId)
        {
            await api.Call(RequestInfo(nameof(Send)), new
            {
                modelId = modelId,
                contractId = contractId,
                invokerId = invokerPlayerId
            });
        }

        [Obsolete("Should not be used in a normal flow")]
        public async Task Return(long modelId)
        {
            await api.Call(RequestInfo(nameof(Return)), new
            {
                modelId = modelId
            });
        }

        [Obsolete("Should not be used in a normal flow")]
        public async Task Unload(long modelId, long companyStorageId, long contractId)
        {
            await api.Call(RequestInfo(nameof(Unload)), new
            {
                modelId = modelId,
                companyStorageId = companyStorageId,
                contractId = contractId
            });
        }

        public async Task Upgrade(long modelId, long invokerPlayerId)
        {
            await SimpleUpgrade(modelId, invokerPlayerId);
        }

        public async Task Upgrade(long modelId, uint stepInSeconds, long invokerPlayerId)
        {
            await api.Call(RequestInfo(nameof(Upgrade)), new
            {
                modelId = modelId,
                stepInSeconds = stepInSeconds,
                invokerId = invokerPlayerId
            });
        }

        public async Task Upgrade(long modelId, uint stepInSeconds)
        {
            await ForceUpgrade(modelId, stepInSeconds);
        }

        public async Task<decimal> GetSpaceOccupiedByItemsForContract(long modelId, long contractId)
        {
            return await api.Call<decimal>(RequestInfo(nameof(GetSpaceOccupiedByItemsForContract)), new
            {
                modelId = modelId,
                contractId = contractId
            });
        }

        //[Obsolete("Should not be used in a normal flow")]
        //public async Task ScheduleUnloadTruck(long modelId, long companyStorageId, long contractId)
        //{
        //    await api.Call<decimal>(RequestInfo(nameof(ScheduleUnloadTruck)), new
        //    {
        //        modelId = modelId,
        //        companyStorageId = companyStorageId,
        //        contractId = contractId
        //    });
        //}

        [Obsolete("Should not be used in a normal flow")]
        public async Task ScheduleReturnTruck(long modelId)
        {
            await api.Call<decimal>(RequestInfo(nameof(ScheduleReturnTruck)), new
            {
                modelId = modelId
            });
        }



        private async Task SimpleUpgrade(long modelId, long invokerPlayerId)
        {
            await api.Call(RequestInfo(nameof(SimpleUpgrade)), new
            {
                modelId = modelId,
                invokerId = invokerPlayerId
            });
        }

        private async Task ForceUpgrade(long modelId, uint stepInSeconds)
        {
            await api.Call(RequestInfo(nameof(ForceUpgrade)), new
            {
                modelId = modelId,
                stepInSeconds = stepInSeconds
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
