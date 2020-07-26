using EnterpriseBot.ApiWrapper.Models.Enums;

namespace EnterpriseBot.ApiWrapper.Models.Game.Localization
{
    public class StringLocalization
    {
        public long Id { get; set; }
        public LocalizationLanguage Language { get; set; }
        public string Text { get; set; }
    }
}
