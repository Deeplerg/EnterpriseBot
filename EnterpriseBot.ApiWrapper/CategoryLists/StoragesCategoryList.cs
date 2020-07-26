using System;
using System.Collections.Generic;
using System.Text;
using EnterpriseBot.ApiWrapper.Categories.Storages;

namespace EnterpriseBot.ApiWrapper.CategoryLists
{
    public class StoragesCategoryList
    {
        internal StoragesCategoryList() { }

        public CompanyStorageCategory CompanyStorage { get; internal set; }
        public InventoryStorageCategory InventoryStorage { get; internal set; }
        public ItemPriceCategory ItemPrice { get; internal set; }
        public ShowcaseStorageCategory ShowcaseStorage { get; internal set; }
        public StorageCategory Storage { get; internal set; }
        public StorageItemCategory StorageItem { get; internal set; }
        public TrunkStorageCategory TrunkStorage { get; internal set; }
    }
}
