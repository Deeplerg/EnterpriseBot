using EnterpriseBot.ApiWrapper.Categories.Money;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseBot.ApiWrapper.CategoryLists
{
    public class MoneyCategoryList
    {
        internal MoneyCategoryList() { }

        public PurseCategory Purse { get; internal set; }
    }
}
