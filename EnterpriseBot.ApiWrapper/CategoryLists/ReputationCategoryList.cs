using EnterpriseBot.ApiWrapper.Categories.Reputation;

namespace EnterpriseBot.ApiWrapper.CategoryLists
{
    public class ReputationCategoryList
    {
        internal ReputationCategoryList() { }

        public ReputationCategory Reputation { get; internal set; }
        public ReviewCategory Review { get; internal set; }
    }
}
