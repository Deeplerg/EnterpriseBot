using EnterpriseBot.ApiWrapper.Abstractions;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories
{
    public abstract class MoneyCategoryBase<TModel, TId, TCreationParams>
                                : ICategory<TModel, TId, TCreationParams> where TModel : class
                                                                          where TCreationParams : class

    {
        protected const string categoryAreaName = "Money";
        protected readonly IApiClient api;

        public MoneyCategoryBase(IApiClient api)
        {
            this.api = api;
        }

        public abstract Task<TModel> Get(TId id);
        public abstract Task<TModel> Create(TCreationParams pars);
    }
}
