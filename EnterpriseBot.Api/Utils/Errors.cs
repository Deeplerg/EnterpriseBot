using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using System;

namespace EnterpriseBot.Api.Utils
{
    public static class Errors
    {
        private const ErrorSeverity DoesNotExistErrorSeverity = ErrorSeverity.Critical;
        private const ErrorSeverity DoesNotHavePermissionErrorSeverity = ErrorSeverity.Normal;
        private const ErrorSeverity IncorrectInputErrorSeverity = ErrorSeverity.Normal;

        #region common errors
        public static readonly LocalizedError StorageCapacityIsMax = new LocalizedError
        {
            ErrorSeverity = ErrorSeverity.Normal,
            EnglishMessage = "Storage capacity is already at the maximum",
            RussianMessage = "Вместимость хранилища уже на максимуме"
        };
        #endregion

        /// <summary>
        /// Returns <see cref="LocalizedError"/> containing info about that the model doesn't exist
        /// </summary>
        /// <typeparam name="T">Generic parameter in <see cref="GameResult{T}"/></typeparam>
        /// <param name="id">Model id</param>
        /// <param name="modelNameEnglish">Model name in English to describe in error</param>
        /// <param name="modelNameRussian">Model name in Russian to describe in error</param>
        /// <returns><see cref="LocalizedError"/> containing info about that model doesn't exist</returns>
        public static LocalizedError DoesNotExist(object id, string modelNameEnglish,
                                                             string modelNameRussian)
        {
            return DoesNotExistLocalizedError(id, modelNameEnglish, modelNameRussian);
        }

        /// <summary>
        /// Returns <see cref="LocalizedError"/> containing info about that the model doesn't exist
        /// </summary>
        /// <typeparam name="T">Generic parameter in <see cref="GameResult{T}"/></typeparam>
        /// <param name="id">Model id</param>
        /// <param name="localization">Localization setting for the model</param>
        /// <returns><see cref="LocalizedError"/> containing info about that model doesn't exist</returns>
        public static LocalizedError DoesNotExist(object id, LocalizationSetting localization)
        {
            return DoesNotExistLocalizedError(id, localization.English, localization.Russian);
        }

        /// <summary>
        /// Returns <see cref="ErrorSeverity.Normal"/> <see cref="LocalizedError"/> containing info about that the player <br/>
        /// does not have a permission to do something (for example, in company)
        /// </summary>
        /// <returns><see cref="LocalizedError"/> containing info about that the player <br/>
        /// does not have a permission to do something</returns>
        public static LocalizedError DoesNotHavePermission()
        {
            return new LocalizedError
            {
                ErrorSeverity = DoesNotHavePermissionErrorSeverity,
                EnglishMessage = "You don't have a permission to do so",
                RussianMessage = "У тебя нет разрешения, чтобы сделать это"
            };
        }

        /// <summary>
        /// Returns <see cref="ErrorSeverity.Normal"/> <see cref="LocalizedError"/> containing info about that the <br/>
        /// <paramref name="enumValue"/> is unknown
        /// </summary>
        /// <typeparam name="T"><see cref="enum"/> which value is unknown</typeparam>
        /// <param name="enumValue"><typeparamref name="T"/> value</param>
        /// <returns><see cref="LocalizedError"/> containing info about that the <br/>
        /// <paramref name="enumValue"/> is unknown</returns>
        public static LocalizedError UnknownEnumValue<T>(T enumValue) where T : Enum
        {
            return new LocalizedError
            {
                ErrorSeverity = ErrorSeverity.Critical,
                EnglishMessage = $"Unknown {typeof(T).Name} value: {enumValue}",
                RussianMessage = $"Неизвестное значение {typeof(T).Name}: {enumValue}"
            };
        }

        #region input
        /// <summary>
        /// Returns <see cref="ErrorSeverity.Normal"/> <see cref="LocalizedError"/> containing info about that the <br/>
        /// inputted <b>name</b> is incorrect
        /// </summary>
        /// <returns><see cref="ErrorSeverity.Normal"/> <see cref="LocalizedError"/></returns>
        public static LocalizedError IncorrectNameInput(UserInputRequirements inputRequirements)
            => IncorrectInput(inputRequirements.Name, Constants.NameMaxLength);

        /// <summary>
        /// Returns <see cref="ErrorSeverity.Normal"/> <see cref="LocalizedError"/> containing info about that the <br/>
        /// inputted <b>business name</b> is incorrect
        /// </summary>
        /// <returns><see cref="ErrorSeverity.Normal"/> <see cref="LocalizedError"/></returns>
        public static LocalizedError IncorrectBusinessNameInput(UserInputRequirements inputRequirements) 
            => IncorrectInput(inputRequirements.BusinessName, Constants.BusinessNameMaxLength);

        /// <summary>
        /// Returns <see cref="ErrorSeverity.Normal"/> <see cref="LocalizedError"/> containing info about that the <br/>
        /// inputted <b>description</b> is incorrect
        /// </summary>
        /// <returns><see cref="ErrorSeverity.Normal"/> <see cref="LocalizedError"/></returns>
        public static LocalizedError IncorrectDescriptionInput(UserInputRequirements inputRequirements)
            => IncorrectInput(inputRequirements.Description, Constants.DescriptionMaxLength);

        /// <summary>
        /// Returns <see cref="ErrorSeverity.Normal"/> <see cref="LocalizedError"/> containing info about that the <br/>
        /// inputted <b>resume</b> is incorrect
        /// </summary>
        /// <returns><see cref="ErrorSeverity.Normal"/> <see cref="LocalizedError"/></returns>
        public static LocalizedError IncorrectResumeInput(UserInputRequirements inputRequirements)
            => IncorrectInput(inputRequirements.Resume, Constants.ResumeMaxLength);

        /// <summary>
        /// Returns <see cref="ErrorSeverity.Normal"/> <see cref="LocalizedError"/> containing info about that the <br/>
        /// inputted <b>review text</b> is incorrect
        /// </summary>
        /// <returns><see cref="ErrorSeverity.Normal"/> <see cref="LocalizedError"/></returns>
        public static LocalizedError IncorrectReviewInput(UserInputRequirements inputRequirements)
            => IncorrectInput(inputRequirements.ReviewText, Constants.ReviewTextMaxLength);

        /// <summary>
        /// Returns <see cref="ErrorSeverity.Normal"/> <see cref="LocalizedError"/> containing info about that the <br/>
        /// inputted <b>password</b> is incorrect
        /// </summary>
        /// <returns><see cref="ErrorSeverity.Normal"/> <see cref="LocalizedError"/></returns>
        public static LocalizedError IncorrectPasswordInput(UserInputRequirements inputRequirements)
            => IncorrectInput(inputRequirements.Password, Constants.PasswordMinLength);

        /// <summary>
        /// Returns <see cref="ErrorSeverity.Normal"/> <see cref="LocalizedError"/> containing info about that the <br/>
        /// inputted <b>player status</b> is incorrect
        /// </summary>
        /// <returns><see cref="ErrorSeverity.Normal"/> <see cref="LocalizedError"/></returns>
        public static LocalizedError IncorrectPlayerStatusInput(UserInputRequirements inputRequirements)
            => IncorrectInput(inputRequirements.PlayerStatus, Constants.PlayerStatusMaxLength);
        #endregion


        /// <summary>
        /// Returns <see cref="LocalizedError"/> with specified <paramref name="id"/> and model name
        /// </summary>
        /// <param name="id">Model id to describe in error</param>
        /// <param name="modelNameEnglish">Model name in English to describe in error</param>
        /// <param name="modelNameRussian">Model name in Russian to describe in error</param>
        /// <returns><see cref="LocalizedError"/> with specified <paramref name="id"/> and model name</returns>
        private static LocalizedError DoesNotExistLocalizedError(object id, string modelNameEnglish,
                                                                            string modelNameRussian)
        {
            return new LocalizedError
            {
                ErrorSeverity = DoesNotExistErrorSeverity,
                EnglishMessage = $"{modelNameEnglish} with id {id} does not exist",
                RussianMessage = $"{modelNameRussian} с id {id} не существует"
            };
        }

        /// <summary>
        /// Returns <see cref="LocalizedError"/> with formatted specified <paramref name="localizationSetting"/> and <paramref name="formatParams"/>
        /// </summary>
        /// <returns><see cref="LocalizedError"/></returns>
        private static LocalizedError IncorrectInput(LocalizationSetting localizationSetting, params object[] formatParams)
        {
            return new LocalizedError
            {
                ErrorSeverity = IncorrectInputErrorSeverity,
                EnglishMessage = string.Format(localizationSetting.English,
                                               formatParams),
                RussianMessage = string.Format(localizationSetting.Russian,
                                               formatParams)
            };
        }
    }
}
