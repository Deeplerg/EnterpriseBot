using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;

namespace EnterpriseBot.Api.Utils
{
    public static class Errors
    {
        private const ErrorSeverity DoesNotExistErrorSeverity = ErrorSeverity.Critical;

        /// <summary>
        /// Returns <see cref="GameResult{T}"/> with <see cref="ErrorSeverity.Critical"/> <see cref="LocalizedError"/> containing info about that the model doesn't exist
        /// </summary>
        /// <typeparam name="T">Generic parameter in <see cref="GameResult{T}"/></typeparam>
        /// <param name="id">Model id</param>
        /// <param name="modelNameEnglish">Model name in English to describe in error</param>
        /// <param name="modelNameRussian">Model name in Russian to describe in error</param>
        /// <returns><see cref="GameResult{T}"/> with <see cref="LocalizedError"/> containing info about that model doesn't exist</returns>
        public static LocalizedError DoesNotExist(object id, string modelNameEnglish,
                                                             string modelNameRussian)
        {
            return GenerateDoesNotExistLocalizedError(id, modelNameEnglish, modelNameRussian);
        }

        /// <summary>
        /// Returns <see cref="GameResult{T}"/> with <see cref="ErrorSeverity.Critical"/> <see cref="LocalizedError"/> containing info about that the model doesn't exist
        /// </summary>
        /// <typeparam name="T">Generic parameter in <see cref="GameResult{T}"/></typeparam>
        /// <param name="id">Model id</param>
        /// <param name="localization">Localization setting for the model</param>
        /// <returns><see cref="GameResult{T}"/> with <see cref="LocalizedError"/> containing info about that model doesn't exist</returns>
        public static LocalizedError DoesNotExist(object id, LocalizationSetting localization)
        {
            return GenerateDoesNotExistLocalizedError(id, localization.English, localization.Russian);
        }


        /// <summary>
        /// Returns <see cref="LocalizedError"/> with specified <paramref name="id"/> and model name
        /// </summary>
        /// <param name="id">Model id to describe in error</param>
        /// <param name="modelNameEnglish">Model name in English to describe in error</param>
        /// <param name="modelNameRussian">Model name in Russian to describe in error</param>
        /// <returns><see cref="LocalizedError"/> with specified <paramref name="id"/> and model name</returns>
        private static LocalizedError GenerateDoesNotExistLocalizedError(object id, string modelNameEnglish,
                                                                                    string modelNameRussian)
        {
            return new LocalizedError
            {
                ErrorSeverity = DoesNotExistErrorSeverity,
                EnglishMessage = $"{modelNameEnglish} with id {id} does not exist",
                RussianMessage = $"{modelNameRussian} с id {id} не существует"
            };
        }
    }
}
