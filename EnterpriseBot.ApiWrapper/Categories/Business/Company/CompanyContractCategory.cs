using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.Game.Business.Company;
using EnterpriseBot.ApiWrapper.Models.Other;
using EnterpriseBot.ApiWrapper.Models.CreationParams.Business.Company;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Business.Company
{
    public class CompanyContractCategory : CompanySubCategoryBase<CompanyContract,
                                                                  long,
                                                                  CompanyContractCreationParams>
    {
        protected const string categoryName = "CompanyContract";

        public CompanyContractCategory(IApiClient apiClient) : base(apiClient) { }

        public override async Task<CompanyContract> Get(long id)
        {
            return await api.Call<CompanyContract>(RequestInfo(nameof(Get)), IdParameter(id));
        }

        public override async Task<CompanyContract> Create(CompanyContractCreationParams pars)
        {
            return await api.Call<CompanyContract>(RequestInfo(nameof(Create)), pars);
        }

        public async Task<CompanyContract> Conclude(long contractRequestId, long invokerPlayerId)
        {
            return await api.Call<CompanyContract>(RequestInfo(nameof(Conclude)), new
            {
                contractRequestId = contractRequestId,
                invokerId = invokerPlayerId
            });
        }


        public async Task<bool> CheckCompletion(long modelId)
        {
            return await api.Call<bool>(RequestInfo(nameof(CheckCompletion)), new
            {
                modelId = modelId
            });
        }

        public async Task<int> AddDeliveredAmount(long modelId, int amount)
        {
            return await api.Call<int>(RequestInfo(nameof(AddDeliveredAmount)), new
            {
                modelId = modelId,
                amount = amount
            });
        }

        public async Task Complete(long modelId)
        {
            await api.Call(RequestInfo(nameof(Complete)), new
            {
                modelId = modelId
            });
        }

        public async Task Break(long modelId)
        {
            await api.Call(RequestInfo(nameof(Break)), new
            {
                modelId = modelId
            });
        }

        //[Obsolete("Should not be used in a normal flow")]
        //public async Task ScheduleCompletionCheck(long modelId)
        //{
        //    await api.Call(RequestInfo(nameof(ScheduleCompletionCheck)), new
        //    {
        //        modelId = modelId
        //    });
        //}



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
