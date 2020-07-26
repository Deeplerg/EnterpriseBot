using EnterpriseBot.ApiWrapper.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories
{
    public abstract class EssencesCategoryBase<TModel, TId, TCreationParams>
                                   : ICategory<TModel, TId, TCreationParams> where TModel : class
                                                                             where TCreationParams : class
    {
        protected const string categoryAreaName = "Essences";
        protected readonly IApiClient api;

        public EssencesCategoryBase(IApiClient apiClient)
        {
            this.api = apiClient;
        }

        public abstract Task<TModel> Get(TId id);
        public abstract Task<TModel> Create(TCreationParams pars);
    }
}
