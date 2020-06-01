using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Categories.Business;

namespace EnterpriseBot.ApiWrapper.CategoryLists
{
    public class BusinessCategoryList : ICategoryList
    {
        public CompanyCategory Company { get; set; }
        public ContractCategory Contract { get; set; }
        public JobCategory Job { get; set; }
        public ShopCategory Shop { get; set; }
    }
}
