using EnterpriseBot.ApiWrapper.Abstractions;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories
{
    public abstract class BusinessCategoryBase<TModel> : ICategory<TModel> where TModel : class
    {
        protected const string categoryAreaName = "business";
        protected readonly IApiClient api;

        public BusinessCategoryBase(IApiClient apiClient)
        {
            api = apiClient;
        }

        public abstract Task<TModel> Get(object id);
    }
}
