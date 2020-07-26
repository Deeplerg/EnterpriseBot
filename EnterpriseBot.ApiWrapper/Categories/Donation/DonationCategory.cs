using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.Game.Donation;
using EnterpriseBot.ApiWrapper.Models.Other;
using EnterpriseBot.ApiWrapper.Models.CreationParams.Donation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EnterpriseBot.ApiWrapper.Models.Enums;

namespace EnterpriseBot.ApiWrapper.Categories.Donation
{
    public class DonationCategory : DonationCategoryBase<Models.Game.Donation.Donation,
                                                         long,
                                                         DonationCreationParams>
    {
        protected const string categoryName = "Donation";

        public DonationCategory(IApiClient apiClient) : base(apiClient) { }

        public override async Task<Models.Game.Donation.Donation> Get(long id)
        {
            return await api.Call<Models.Game.Donation.Donation>(RequestInfo(nameof(Get)), IdParameter(id));
        }

        public override async Task<Models.Game.Donation.Donation> Create(DonationCreationParams pars)
        {
            return await api.Call<Models.Game.Donation.Donation>(RequestInfo(nameof(Create)), pars);
        }


        public async Task<decimal> GetBusinessPriceMultiplierForPrivilege(Privilege privilege)
        {
            return await api.Call<decimal>(RequestInfo(nameof(GetBusinessPriceMultiplierForPrivilege)), new
            {
                privilege = privilege
            });
        }

        public async Task<decimal> GetBusinessPriceMultiplier(long modelId)
        {
            return await api.Call<decimal>(RequestInfo(nameof(GetBusinessPriceMultiplier)), new
            {
                modelId = modelId
            });
        }

        public async Task<uint> GetMaxContractsForPrivilege(Privilege privilege)
        {
            return await api.Call<uint>(RequestInfo(nameof(GetMaxContractsForPrivilege)), new
            {
                privilege = privilege
            });
        }

        public async Task<uint> GetMaxContracts(long modelId)
        {
            return await api.Call<uint>(RequestInfo(nameof(GetMaxContracts)), new
            {
                modelId = modelId
            });
        }

        public async Task<uint> GetContractMaxTimeInDaysForPrivilege(Privilege privilege)
        {
            return await api.Call<uint>(RequestInfo(nameof(GetContractMaxTimeInDaysForPrivilege)), new
            {
                privilege = privilege
            });
        }

        public async Task<uint> GetContractMaxTimeInDays(long modelId)
        {
            return await api.Call<uint>(RequestInfo(nameof(GetContractMaxTimeInDays)), new
            {
                modelId = modelId
            });
        }

        public async Task<Privilege> UpgradePrivilege(long modelId, Privilege privilege)
        {
            return await api.Call<Privilege>(RequestInfo(nameof(UpgradePrivilege)), new
            {
                modelId = modelId,
                privilege = privilege
            });
        }

        public async Task<DonationPurchase> AddPurchase(long modelId, DonationPurchaseCreationParams pars)
        {
            return await api.Call<DonationPurchase>(RequestInfo(nameof(AddPurchase)), new
            {
                modelId = modelId,
                donationPurchaseParams = pars
            });
        }

        public async Task<bool> CanUpgradeToPrivilege(long modelId, Privilege privilege)
        {
            return await api.Call<bool>(RequestInfo(nameof(CanUpgradeToPrivilege)), new
            {
                modelId = modelId,
                privilege = privilege
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
