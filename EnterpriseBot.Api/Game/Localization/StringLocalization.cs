using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.ModelCreationParams.Localization;
using EnterpriseBot.Api.Models.Other;

namespace EnterpriseBot.Api.Game.Localization
{
    public class StringLocalization
    {
        #region model
        public long Id { get; protected set; }

        public LocalizationLanguage Language { get; protected set; }
        public string Text { get; protected set; }

        #region errors
        private static readonly LocalizedError textEmptyOrWhiteSpaceError = new LocalizedError
        {
            ErrorSeverity = ErrorSeverity.Normal,
            EnglishMessage = "The text cannot be empty or contain only space characters",
            RussianMessage = "Текст не может быть пусВым или состоять только из пробелов"
        };
        #endregion
        #endregion

        #region actions
        public static GameResult<StringLocalization> Create(StringLocalizationCreationParams pars)
        {
            if (string.IsNullOrWhiteSpace(pars.Text))
            {
                return textEmptyOrWhiteSpaceError;
            }

            return new StringLocalization
            {
                Text = pars.Text,
                Language = pars.Language
            };
        }

        public GameResult<string> SetText(string newText)
        {
            if (string.IsNullOrWhiteSpace(newText))
            {
                return textEmptyOrWhiteSpaceError;
            }

            Text = newText;

            return Text;
        }

        public GameResult<LocalizationLanguage> SetLanguage(LocalizationLanguage newLanguage)
        {
            Language = newLanguage;

            return Language;
        }
        #endregion
    }
}
