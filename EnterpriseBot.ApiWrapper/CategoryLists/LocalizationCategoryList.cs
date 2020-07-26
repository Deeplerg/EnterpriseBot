using EnterpriseBot.ApiWrapper.Categories.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseBot.ApiWrapper.CategoryLists
{
    public class LocalizationCategoryList
    {
        internal LocalizationCategoryList() { }

        public LocalizedStringCategory LocalizedString { get; internal set; }
        public StringLocalizationCategory StringLocalization { get; internal set; }
    }
}
