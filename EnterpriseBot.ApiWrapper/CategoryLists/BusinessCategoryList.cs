using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseBot.ApiWrapper.CategoryLists
{
    public class BusinessCategoryList
    {
        internal BusinessCategoryList() { }

        public CompanySubCategoryList Company { get; internal set; }
    }
}
