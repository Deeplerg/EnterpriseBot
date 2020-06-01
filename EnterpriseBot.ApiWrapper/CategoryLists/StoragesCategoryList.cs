using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Categories.Storages;

namespace EnterpriseBot.ApiWrapper.CategoryLists
{
    public class StoragesCategoryList : ICategoryList
    {
        public IncomeStorageCategory IncomeStorage { get; set; }
        public OutcomeStorageCategory OutcomeStorage { get; set; }
        public WorkerStorageCategory WorkerStorage { get; set; }

        public TrunkStorageCategory TrunkStorage { get; set; }
        public ShowcaseStorageCategory ShowcaseStorage { get; set; }

        public PersonalStorageCategory PersonalStorage { get; set; }
    }
}
