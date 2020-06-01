using EnterpriseBot.ApiWrapper.Abstractions;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories
{
    public abstract class StoragesCategoryBase<TModel> : ICategory<TModel> where TModel : class
    {
        protected const string categoryAreaName = "storages";
        protected readonly IApiClient api;

        public StoragesCategoryBase(IApiClient apiClient)
        {
            api = apiClient;
        }

        public abstract Task<TModel> Get(object id);
    }
}
