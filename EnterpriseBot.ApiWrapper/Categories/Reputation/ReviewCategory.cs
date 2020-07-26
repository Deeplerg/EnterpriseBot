using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.CreationParams.Reputation;
using EnterpriseBot.ApiWrapper.Models.Game.Reputation;
using EnterpriseBot.ApiWrapper.Models.Other;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Reputation
{
    public class ReviewCategory : ReputationCategoryBase<Review, long, ReviewCreationParams>
    {
        protected const string categoryName = "Review";

        internal ReviewCategory(IApiClient api) : base(api) { } 

        public override async Task<Review> Get(long id)
        {
            return await api.Call<Review>(RequestInfo(nameof(Get)), IdParameter(id));
        }

        public override async Task<Review> Create(ReviewCreationParams pars)
        {
            return await api.Call<Review>(RequestInfo(nameof(Create)), pars);
        }


        public async Task<Review> Edit(long modelId, string newText, sbyte newRating)
        {
            return await api.Call<Review>(RequestInfo(nameof(Edit)), new
            {
                modelId = modelId,
                newText = newText,
                newRating = newRating
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
