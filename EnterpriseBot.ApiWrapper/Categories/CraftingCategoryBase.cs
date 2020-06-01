using EnterpriseBot.ApiWrapper.Abstractions;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories
{
    public abstract class CraftingCategoryBase<TModel> : ICategory<TModel> where TModel : class
    {
        protected const string categoryAreaName = "crafting";
        protected readonly IApiClient api;

        public CraftingCategoryBase(IApiClient apiClient)
        {
            api = apiClient;
        }

        public abstract Task<TModel> Get(object id);
    }
}
