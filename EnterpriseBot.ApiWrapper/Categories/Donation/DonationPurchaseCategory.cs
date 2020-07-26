using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.CreationParams.Donation;
using EnterpriseBot.ApiWrapper.Models.Game.Donation;
using EnterpriseBot.ApiWrapper.Models.Other;
using System;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Donation
{
    public class DonationPurchaseCategory : DonationCategoryBase<DonationPurchase,
                                                                 long,
                                                                 DonationPurchaseCreationParams>
    {
        protected const string categoryName = "DonationPurchase";

        public DonationPurchaseCategory(IApiClient apiClient) : base(apiClient) { }

        public override async Task<DonationPurchase> Get(long id)
        {
            return await api.Call<DonationPurchase>(RequestInfo(nameof(Get)), IdParameter(id));
        }

        [Obsolete("Should not be used in a normal flow")]
        public override async Task<DonationPurchase> Create(DonationPurchaseCreationParams pars)
        {
            return await api.Call<DonationPurchase>(RequestInfo(nameof(Create)), pars);
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
