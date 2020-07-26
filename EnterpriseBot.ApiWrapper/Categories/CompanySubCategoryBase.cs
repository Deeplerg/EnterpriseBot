using EnterpriseBot.ApiWrapper.Abstractions;

namespace EnterpriseBot.ApiWrapper.Categories
{
    public abstract class CompanySubCategoryBase<TModel, TId, TCreationParams>
                          : BusinessCategoryBase<TModel, TId, TCreationParams> where TModel : class
                                                                               where TCreationParams : class
    {
        protected const string categorySubAreaName = "Company";

        public CompanySubCategoryBase(IApiClient apiClient) : base(apiClient) { }
    }
}
