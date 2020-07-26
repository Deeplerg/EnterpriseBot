using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.Game.Business.Company;
using EnterpriseBot.ApiWrapper.Models.Other;
using EnterpriseBot.ApiWrapper.Models.CreationParams.Business.Company;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EnterpriseBot.ApiWrapper.Models.Game.Essences;

namespace EnterpriseBot.ApiWrapper.Categories.Business.Company
{
    public class TruckGarageCategory : CompanySubCategoryBase<TruckGarage,
                                                              long,
                                                              TruckGarageCreationParams>
    {
        protected const string categoryName = "TruckGarage";

        public TruckGarageCategory(IApiClient apiClient) : base(apiClient) { }

        public override async Task<TruckGarage> Get(long id)
        {
            return await api.Call<TruckGarage>(RequestInfo(nameof(Get)), IdParameter(id));
        }

        public override async Task<TruckGarage> Create(TruckGarageCreationParams pars)
        {
            return await api.Call<TruckGarage>(RequestInfo(nameof(Create)), pars);
        }


        public async Task<Truck> AddTruck(long modelId, TruckCreationParams pars)
        {
            return await api.Call<Truck>(RequestInfo(nameof(AddTruck)), new
            {
                modelId = modelId,
                truckCreationParams = pars
            });
        }

        public async Task<Truck> BuyAndAddTruck(long modelId, TruckCreationParams pars, long invokerPlayerId)
        {
            return await api.Call<Truck>(RequestInfo(nameof(BuyAndAddTruck)), new
            {
                modelId = modelId,
                truckCreationParams = pars,
                invokerId = invokerPlayerId
            });
        }

        public async Task<sbyte> Upgrade(long modelId, long invokerPlayerId)
        {
            return await api.Call<sbyte>(RequestInfo(nameof(Upgrade)), new
            {
                modelId = modelId,
                invokerId = invokerPlayerId
            });
        }

        public async Task<sbyte> ForceUpgrade(long modelId, sbyte step)
        {
            return await api.Call<sbyte>(RequestInfo(nameof(ForceUpgrade)), new
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
