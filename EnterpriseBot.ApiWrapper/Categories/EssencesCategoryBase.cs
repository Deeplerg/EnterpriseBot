using EnterpriseBot.ApiWrapper.Abstractions;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories
{
    public abstract class EssencesCategoryBase<TModel> : ICategory<TModel> where TModel : class
    {
        protected const string categoryAreaName = "essences";
        protected readonly IApiClient api;

        public EssencesCategoryBase(IApiClient apiClient)
        {
            api = apiClient;
        }

        public abstract Task<TModel> Get(object id);
    }
}
