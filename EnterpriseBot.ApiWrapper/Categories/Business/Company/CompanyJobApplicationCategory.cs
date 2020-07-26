using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.CreationParams.Business.Company;
using EnterpriseBot.ApiWrapper.Models.Game.Business.Company;
using EnterpriseBot.ApiWrapper.Models.Other;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Business.Company
{
    public class CompanyJobApplicationCategory : CompanySubCategoryBase<CompanyJobApplication,
                                                                        long,
                                                                        CompanyJobApplicationCreationParams>
    {
        protected const string categoryName = "CompanyJobApplication";

        public CompanyJobApplicationCategory(IApiClient apiClient) : base(apiClient) { }

        public override async Task<CompanyJobApplication> Get(long id)
        {
            return await api.Call<CompanyJobApplication>(RequestInfo(nameof(Get)), IdParameter(id));
        }

        public override async Task<CompanyJobApplication> Create(CompanyJobApplicationCreationParams pars)
        {
            return await api.Call<CompanyJobApplication>(RequestInfo(nameof(Create)), pars);
        }


        public async Task Decline(long modelId)
        {
            await api.Call(RequestInfo(nameof(Decline)), new
            {
                modelId = modelId
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
