using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.CreationParams.Business;
using EnterpriseBot.ApiWrapper.Models.CreationParams.Business.Company;
using EnterpriseBot.ApiWrapper.Models.Enums;
using EnterpriseBot.ApiWrapper.Models.Game.Localization;
using EnterpriseBot.ApiWrapper.Models.Game.Reputation;
using EnterpriseBot.ApiWrapper.Models.Game.Storages;
using EnterpriseBot.ApiWrapper.Models.Other;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Business.Company
{
    public class CompanyCategory : CompanySubCategoryBase<Models.Game.Business.Company.Company,
                                                          long,
                                                          CompanyCreationParams>
    {
        protected const string categoryName = "Company";

        public CompanyCategory(IApiClient apiClient) : base(apiClient) { }

        public override async Task<Models.Game.Business.Company.Company> Get(long id)
        {
            return await api.Call<Models.Game.Business.Company.Company>(RequestInfo(nameof(Get)), IdParameter(id));
        }

        public override async Task<Models.Game.Business.Company.Company> Create(CompanyCreationParams pars)
        {
            return await api.Call<Models.Game.Business.Company.Company>(RequestInfo(nameof(Create)), pars);
        }


        public async Task<Models.Game.Business.Company.Company> BuyAndCreate(long ownerPlayerId, CompanyCreationParams pars)
        {
            return await api.Call(RequestInfo(nameof(BuyAndCreate)), new
            {
                ownerId = ownerPlayerId,
                companyCreationParams = pars
            });
        }

        public async Task<decimal> GetOverallCreationPrice(CompanyExtensions extensions, long ownerPlayerId)
        {
            return await api.Call<decimal>(RequestInfo(nameof(GetOverallCreationPrice)), new
            {
                extensions = extensions,
                ownerId = ownerPlayerId
            });
        }

        public async Task<CompanyStorage> GetCompanyStorageWithAvailableSpace(long modelId, decimal space, CompanyStorageType storageType)
        {
            return await api.Call<CompanyStorage>(RequestInfo(nameof(GetCompanyStorageWithAvailableSpace)), new
            {
                modelId = modelId,
                space = space,
                storageType = storageType
            });
        }

        public async Task<bool> HasCompanyStorageWithAvailableSpace(long modelId, decimal space, CompanyStorageType storageType)
        {
            return await api.Call<bool>(RequestInfo(nameof(HasCompanyStorageWithAvailableSpace)), new
            {
                modelId = modelId,
                space = space,
                storageType = storageType
            });
        }

        public async Task<decimal> ReduceBusinessCoins(long modelId, decimal amount)
        {
            return await ReduceBusinessCoins(modelId, amount, invokerId: null);
        }

        public async Task<decimal> ReduceBusinessCoins(long modelId, decimal amount, long invokerPlayerId)
        {
            return await ReduceBusinessCoins(modelId, amount, invokerId: invokerPlayerId);
        }

        public async Task<StringLocalization> EditDescription(long modelId, string newDescription, LocalizationLanguage localizationLanguage)
        {
            return await api.Call<StringLocalization>(RequestInfo(nameof(EditDescription)), new
            {
                modelId = modelId,
                newDescription = newDescription,
                language = localizationLanguage
            });
        }

        public async Task SetOwner(long modelId, long newOwnerPlayerId)
        {
            await api.Call(RequestInfo(nameof(SetOwner)), new
            {
                modelId = modelId,
                newOwnerPlayerId = newOwnerPlayerId
            });
        }

        //public async Task CompleteAndRemoveContract(long modelId, long contractId)
        //{
        //    await api.Call(RequestInfo(nameof(CompleteAndRemoveContract)), new
        //    {
        //        modelId = modelId,
        //        contractId = contractId
        //    });
        //}

        public async Task<bool> CanConcludeOneMoreContract(long modelId)
        {
            return await CanConcludeOneMoreContract(modelId, invokerId: null);
        }

        public async Task<bool> CanConcludeOneMoreContract(long modelId, long invokerPlayerId)
        {
            return await CanConcludeOneMoreContract(modelId, invokerId: invokerPlayerId);
        }

        public async Task<uint> GetContractMaxTimeInDays(long modelId)
        {
            return await GetContractMaxTimeInDays(modelId, invokerId: null);
        }

        public async Task<uint> GetContractMaxTimeInDays(long modelId, long invokerPlayerId)
        {
            return await GetContractMaxTimeInDays(modelId, invokerId: invokerPlayerId);
        }

        public async Task<Review> WriteReview(long modelId, string text, sbyte rating, long reputationId)
        {
            return await WriteReview(modelId, text, rating, reputationId, invokerId: null);
        }

        public async Task<Review> WriteReview(long modelId, string text, sbyte rating, long reputationId, long invokerPlayerId)
        {
            return await WriteReview(modelId, text, rating, reputationId, invokerId: invokerPlayerId);
        }

        public async Task<Review> EditReview(long modelId, long reviewId, string newText, sbyte newRating)
        {
            return await EditReview(modelId, reviewId, newText, newRating, invokerId: null);
        }

        public async Task<Review> EditReview(long modelId, long reviewId, string newText, sbyte newRating, long invokerPlayerId)
        {
            return await EditReview(modelId, reviewId, newText, newRating, invokerId: invokerPlayerId);
        }


        private async Task<Review> WriteReview(long modelId, string text, sbyte rating, long reputationId, long? invokerId)
        {
            return await api.Call<Review>(RequestInfo(nameof(WriteReview)), new
            {
                modelId = modelId,
                text = text,
                rating = rating,
                reputationId = reputationId,
                invokerId = invokerId
            });
        }

        private async Task<Review> EditReview(long modelId, long reviewId, string newText, sbyte newRating, long? invokerId)
        {
            return await api.Call<Review>(RequestInfo(nameof(WriteReview)), new
            {
                modelId = modelId,
                reviewId = reviewId,
                newText = newText,
                newRating = newRating,
                invokerId = invokerId
            });
        }

        private async Task<uint> GetContractMaxTimeInDays(long modelId, long? invokerId)
        {
            return await api.Call<uint>(RequestInfo(nameof(GetContractMaxTimeInDays)), new
            {
                modelId = modelId,
                invokerId = invokerId
            });
        }

        private async Task<bool> CanConcludeOneMoreContract(long modelId, long? invokerId)
        {
            return await api.Call<bool>(RequestInfo(nameof(CanConcludeOneMoreContract)), new
            {
                modelId = modelId,
                invokerId = invokerId
            });
        }

        private async Task<decimal> ReduceBusinessCoins(long modelId, decimal amount, long? invokerId)
        {
            return await api.Call<decimal>(RequestInfo(nameof(ReduceBusinessCoins)), new
            {
                modelId = modelId,
                amount = amount,
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
