namespace EnterpriseBot.Api.Models.Settings.LocalizationSettings //!!! Settings.LocalizationSettings
{
    public class LocalizationSettings
    {
        public UserInputRequirements UserInputRequirements { get; set; }
        public BusinessLocalizationSettings Business { get; set; }
        public CraftingLocalizationSettings Crafting { get; set; }
        public DonationLocalizationSettings Donation { get; set; }
        public EssencesLocalizationSettings Essences { get; set; }
        public LocalizationLocalizationSettings Localization { get; set; }
        public MoneyLocalizationSettings Money { get; set; }
        public ReputationLocalizationSettings Reputation { get; set; }
        public StoragesLocalizationSettings Storages { get; set; }
    }
}
