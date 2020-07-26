using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.Game.Business.Company;
using EnterpriseBot.ApiWrapper.Models.Other;
using EnterpriseBot.ApiWrapper.Models.CreationParams.Business.Company;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EnterpriseBot.ApiWrapper.Models.Enums;
using EnterpriseBot.ApiWrapper.Models.Game.Localization;
using Newtonsoft.Json;
using System.Reflection.Metadata.Ecma335;

namespace EnterpriseBot.ApiWrapper.Categories.Business.Company
{
    public class CompanyJobCategory : CompanySubCategoryBase<CompanyJob,
                                                             long,
                                                             CompanyJobCreationParams>
    {
        protected const string categoryName = "CompanyJob";

        public CompanyJobCategory(IApiClient apiClient) : base(apiClient) { }

        public override async Task<CompanyJob> Get(long id)
        {
            return await api.Call<CompanyJob>(RequestInfo(nameof(Get)), IdParameter(id));
        }

        public override async Task<CompanyJob> Create(CompanyJobCreationParams pars)
        {
            return await api.Call<CompanyJob>(RequestInfo(nameof(Create)), pars);
        }


        public async Task<StringLocalization> SetDescription(long modelId, 
                                                             string newDescription, 
                                                             LocalizationLanguage localizationLanguage, 
                                                             long invokerPlayerId)
        {
            return await api.Call<StringLocalization>(RequestInfo(nameof(SetDescription)), new
            {
                modelId = modelId,
                newDescription = newDescription,
                language = localizationLanguage,
                invokerId = invokerPlayerId
            });
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

        public async Task<CompanyJobApplication> Apply(long modelId, long applicantPlayerId, string resume)
        {
            return await api.Call<CompanyJobApplication>(RequestInfo(nameof(Apply)), new
            {
                modelId = modelId,
                applicantId = applicantPlayerId,
                resume = resume
            });
        }

        public async Task<CompanyJob> Hire(long modelId, long jobApplicationId, long invokerPlayerId)
        {
            return await api.Call<CompanyJob>(RequestInfo(nameof(Hire)), new
            {
                modelId = modelId,
                applicationId = jobApplicationId,
                invokerId = invokerPlayerId
            });
        }

        public async Task<CompanyJob> Fire(long modelId)
        {
            return await Fire(modelId, invokerId: null);
        }

        public async Task<CompanyJob> Fire(long modelId, long invokerPlayerId)
        {
            return await Fire(modelId, invokerId: invokerPlayerId);
        }

        [Obsolete("Should not be used in a normal flow")]
        public async Task PaySalary(long modelId)
        {
            await api.Call(RequestInfo(nameof(PaySalary)), new
            {
                modelId = modelId
            });
        }

        public async Task<CompanyJobPermissions> AddPermissions(long modelId, 
                                                                CompanyJobPermissions newPermissions, 
                                                                long invokerPlayerId)
        {
            return await api.Call<CompanyJobPermissions>(RequestInfo(nameof(AddPermissions)), new
            {
                modelId = modelId,
                newPermissions = newPermissions,
                invokerId = invokerPlayerId
            });
        }

        public async Task<CompanyJobPermissions> RemovePermissions(long modelId,
                                                                   CompanyJobPermissions permissionsToRemove,
                                                                   long invokerPlayerId)
        {
            return await api.Call<CompanyJobPermissions>(RequestInfo(nameof(RemovePermissions)), new
            {
                modelId = modelId,
                permissionsToRemove = permissionsToRemove,
                invokerId = invokerPlayerId
            });
        }

        public async Task<CompanyJobPermissions> SetPermissions(long modelId,
                                                                CompanyJobPermissions newPermissions,
                                                                long invokerPlayerId)
        {
            return await api.Call<CompanyJobPermissions>(RequestInfo(nameof(SetPermissions)), new
            {
                modelId = modelId,
                newPermissions = newPermissions,
                invokerId = invokerPlayerId
            });
        }

        public async Task<decimal> SetSalary(long modelId,
                                             decimal newSalary,
                                             long invokerPlayerId)
        {
            return await api.Call<decimal>(RequestInfo(nameof(SetSalary)), new
            {
                modelId = modelId,
                newSalary = newSalary,
                invokerId = invokerPlayerId
            });
        }

        public async Task<bool> DoesJobHavePermissions(long modelId,
                                                      CompanyJobPermissions permissions)
        {
            return await api.Call<bool>(RequestInfo("DoesJobHavePermission"), new
            {
                modelId = modelId,
                permission = permissions
            });
        }

        [Obsolete("Should not be used in a normal flow")]
        public async Task SchedulePaySalary(long modelId)
        {
            await api.Call(RequestInfo(nameof(SchedulePaySalary)), new
            {
                modelId = modelId
            });
        }



        private async Task<CompanyJob> Fire(long modelId, long? invokerId)
        {
            return await api.Call<CompanyJob>(RequestInfo(nameof(Fire)), new
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
