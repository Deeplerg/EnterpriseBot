using EnterpriseBot.ApiWrapper.Categories.Localization;

namespace EnterpriseBot.ApiWrapper.CategoryLists
{
    public class LocalizationCategoryList
    {
        internal LocalizationCategoryList() { }

        public LocalizedStringCategory LocalizedString { get; internal set; }
        public StringLocalizationCategory StringLocalization { get; internal set; }
    }
}
