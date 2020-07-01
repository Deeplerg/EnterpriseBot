namespace EnterpriseBot.Api.Models.Settings.LocalizationSettings
{
    public class CraftingLocalizationSettings
    {
        public LocalizationSetting CraftingCategory { get; set; }
        public LocalizationSetting CraftingSubCategory { get; set; }
        public LocalizationSetting Ingredient { get; set; }
        public LocalizationSetting Item { get; set; }
        public LocalizationSetting Recipe { get; set; }
    }
}
