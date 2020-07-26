using System.Collections.Generic;

namespace EnterpriseBot.ApiWrapper.Models.Game.Localization
{
    public class LocalizedString
    {
        public long Id { get; set; }
        public List<StringLocalization> Localizations { get; set; } = new List<StringLocalization>();
    }
}
