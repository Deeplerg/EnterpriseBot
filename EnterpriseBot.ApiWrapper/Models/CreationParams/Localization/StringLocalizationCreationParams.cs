using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnterpriseBot.ApiWrapper.Models.Enums;

namespace EnterpriseBot.ApiWrapper.Models.CreationParams.Localization
{
    public class StringLocalizationCreationParams
    {
        public string Text { get; set; }
        public LocalizationLanguage Language { get; set; }
    }
}
