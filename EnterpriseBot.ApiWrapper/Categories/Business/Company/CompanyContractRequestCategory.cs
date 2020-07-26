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
    public class CompanyContractRequestCategory : CompanySubCategoryBase<CompanyContractRequest,
                                                                         long,
                                                                         CompanyContractRequestCreationParams>
    {
        protected const string categoryName = "CompanyContractRequest";

        public CompanyContractRequestCategory(IApiClient apiClient) : base(apiClient) { }

        public override async Task<CompanyContractRequest> Get(long id)
        {
            return await api.Call<CompanyContractRequest>(RequestInfo(nameof(Get)), IdParameter(id));
        }

        public override async Task<CompanyContractRequest> Create(CompanyContractRequestCreationParams pars)
        {
            return await api.Call<CompanyContractRequest>(RequestInfo(nameof(Create)), pars);
        }


        public async Task Decline(long modelId)
        {
            await api.Call(RequestInfo(nameof(Decline)), new
            {
                modelId = modelId
            });
        }

        public async Task<string> SetName(long modelId, string newName)
        {
            return await api.Call(RequestInfo(nameof(SetName)), new
            {
                modelId = modelId,
                newName = newName
            });
        }

        public async Task<string> SetDescription(long modelId, string newDescription)
        {
            return await api.Call(RequestInfo(nameof(SetDescription)), new
            {
                modelId = modelId,
                newDescription = newDescription
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

