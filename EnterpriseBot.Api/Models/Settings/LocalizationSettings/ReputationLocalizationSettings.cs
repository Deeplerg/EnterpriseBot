using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.Settings.LocalizationSettings
{
    public class ReputationLocalizationSettings
    {
        public LocalizationSetting Reputation { get; set; }
        public LocalizationSetting Review { get; set; }
    }
}
