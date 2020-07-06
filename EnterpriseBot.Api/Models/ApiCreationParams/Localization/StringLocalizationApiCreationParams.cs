using EnterpriseBot.Api.Game.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.ApiCreationParams.Localization
{
    public class StringLocalizationApiCreationParams
    {
        public string Text { get; set; }
        public LocalizationLanguage Language { get; set; }
    }
}
