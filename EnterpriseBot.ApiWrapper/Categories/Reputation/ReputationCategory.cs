using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.CreationParams.Reputation;
using EnterpriseBot.ApiWrapper.Models.Game.Reputation;
using EnterpriseBot.ApiWrapper.Models.Other;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Reputation
{
    public class ReputationCategory : ReputationCategoryBase<Models.Game.Reputation.Reputation,
                                                             long,
                                                             ReputationCreationParams>
    {
        protected const string categoryName = "Reputation";

        public ReputationCategory(IApiClient api) : base(api) { }

        public override async Task<Models.Game.Reputation.Reputation> Get(long id)
        {
            return await api.Call<Models.Game.Reputation.Reputation>(RequestInfo(nameof(Get)), IdParameter(id));
        }

        public override async Task<Models.Game.Reputation.Reputation> Create(ReputationCreationParams pars)
        {
            return await api.Call<Models.Game.Reputation.Reputation>(RequestInfo(nameof(Create)), pars);
        }


        public async Task<Review> AddReview(long modelId, ReviewCreationParams reviewParams)
        {
            return await api.Call<Review>(RequestInfo(nameof(AddReview)), new
            {
                modelId = modelId,
                reviewParams = reviewParams
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
