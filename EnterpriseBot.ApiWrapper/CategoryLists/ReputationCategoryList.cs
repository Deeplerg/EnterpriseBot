using EnterpriseBot.ApiWrapper.Categories.Reputation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseBot.ApiWrapper.CategoryLists
{
    public class ReputationCategoryList
    {
        internal ReputationCategoryList() { }

        public ReputationCategory Reputation { get; internal set; }
        public ReviewCategory Review { get; internal set; }
    }
}
