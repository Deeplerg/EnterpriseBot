using EnterpriseBot.Api.Models.ModelCreationParams.Localization;
using EnterpriseBot.Api.Models.Other;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Game.Localization
{
    public class LocalizedString
    {
        protected LocalizedString() { }

        #region model
        public long Id { get; protected set; }

        public virtual IReadOnlyCollection<StringLocalization> Localizations
        {
            get => new ReadOnlyCollection<StringLocalization>(localizations);
            protected set => localizations = value.ToList();
        }
        private List<StringLocalization> localizations = new List<StringLocalization>();

        #region errors
        private static readonly LocalizedError localizationIsNotPresentError = new LocalizedError
        {
            ErrorSeverity = ErrorSeverity.Normal,
            EnglishMessage = "The localization for this language is not present",
            RussianMessage = "Локализации для этого языка нет"
        };
        #endregion
        #endregion

        #region actions
        public static GameResult<LocalizedString> Create(LocalizedStringCreationParams pars)
        {
            var localizedString = new LocalizedString
            {
            };

            if (pars.Localizations != null && pars.Localizations.Any()) 
                localizedString.Localizations = pars.Localizations;

            return localizedString;
        }

        public GameResult<IEnumerable<StringLocalization>> Add(StringLocalizationCreationParams localizationParams)
        {
            if(localizations.Any(loc => loc.Language == localizationParams.Language))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The text is already translated to this language",
                    RussianMessage = "Текст уже переведён на этот язык"
                };
            }

            var stringLocalizationCreationResult = StringLocalization.Create(localizationParams);
            if (stringLocalizationCreationResult.LocalizedError != null) return stringLocalizationCreationResult.LocalizedError;
            
            StringLocalization stringLocalization = stringLocalizationCreationResult;

            localizations.Add(stringLocalization);

            return localizations;
        }

        public GameResult<StringLocalization> Edit(string newText, LocalizationLanguage language)
        {
            var getStringLocalizationResult = GetLocalization(language);
            if (getStringLocalizationResult.LocalizedError != null) return getStringLocalizationResult.LocalizedError;

            StringLocalization stringLocalization = getStringLocalizationResult;

            var setResult = stringLocalization.SetText(newText);
            if (setResult.LocalizedError != null) return setResult.LocalizedError;

            return stringLocalization;
        }

        public GameResult<StringLocalization> GetLocalization(LocalizationLanguage language)
        {
            if(!IsLocalizationPresent(language))
            {
                return localizationIsNotPresentError;
            }

            return localizations.Single(loc => loc.Language == language);
        }

        public bool IsLocalizationPresent(LocalizationLanguage language)
        {
            return localizations.Any(loc => loc.Language == language);
        }
        #endregion
    }
}
