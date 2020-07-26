using EnterpriseBot.ApiWrapper.Abstractions;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories
{
    public abstract class DonationCategoryBase<TModel, TId, TCreationParams>
                                   : ICategory<TModel, TId, TCreationParams> where TModel : class
                                                                             where TCreationParams : class
    {
        protected const string categoryAreaName = "Donation";
        protected readonly IApiClient api;

        public DonationCategoryBase(IApiClient apiClient)
        {
            this.api = apiClient;
        }

        public abstract Task<TModel> Get(TId id);
        public abstract Task<TModel> Create(TCreationParams pars);
    }
}
