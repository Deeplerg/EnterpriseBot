using EnterpriseBot.Api.Game.Localization;
using System.Collections.Generic;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Localization
{
    public class LocalizedStringCreationParams
    {
        /// <summary>
        /// Optional
        /// </summary>
        public List<StringLocalization> Localizations { get; set; }
    }
}
