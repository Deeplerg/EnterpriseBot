using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.Settings.LocalizationSettings
{
    public class LocalizationLocalizationSettings
    {
        public LocalizationSetting LocalizationLanguage { get; set; }
        public LocalizationSetting LocalizedString { get; set; }
        public LocalizationSetting StringLocalization { get; set; }
    }
}
