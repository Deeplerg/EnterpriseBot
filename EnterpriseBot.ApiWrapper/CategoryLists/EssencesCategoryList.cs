using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Categories.Essences;

namespace EnterpriseBot.ApiWrapper.CategoryLists
{
    public class EssencesCategoryList : ICategoryList
    {
        public PlayerCategory Player { get; set; }
        public BotCategory Bot { get; set; }
    }
}
