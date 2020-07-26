using EnterpriseBot.ApiWrapper.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories
{
    public abstract class ReputationCategoryBase<TModel, TId, TCreationParams>
                                     : ICategory<TModel, TId, TCreationParams> where TModel : class
                                                                               where TCreationParams : class
    {
        protected const string categoryAreaName = "Reputation";
        protected readonly IApiClient api;

        public ReputationCategoryBase(IApiClient apiClient)
        {
            this.api = apiClient;
        }

        public abstract Task<TModel> Get(TId id);
        public abstract Task<TModel> Create(TCreationParams pars);
    }
}
