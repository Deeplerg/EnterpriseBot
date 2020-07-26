using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Game.Storages;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.ModelCreationParams.Business;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings;
using Newtonsoft.Json;
using System;

namespace EnterpriseBot.Api.Game.Business.Company
{
    public class CompanyWorker
    {
        protected CompanyWorker() { }

        #region model
        public long Id { get; protected set; }

        public virtual Company Company { get; protected set; }

        public virtual Recipe Recipe { get; protected set; }

        public virtual CompanyStorage WorkingStorage { get; protected set; }

        public bool IsWorkingNow { get; protected set; }

        public decimal SpeedMultiplier { get; protected set; }

        [JsonIgnore]
        public string ProduceItemAndStopJobId { get; set; }

        public int LeadTimeInSeconds
        {
            get => (int)Math.Floor(Recipe.LeadTimeInSeconds / SpeedMultiplier);
        }
        #endregion

        #region actions
        public static GameResult<CompanyWorker> Create(CompanyWorkerCreationParams pars, GameSettings gameSettings)
        {
            var workerCreationResult = CreateBase(pars, gameSettings);
            if (workerCreationResult.LocalizedError != null) return workerCreationResult.LocalizedError;

            var worker = workerCreationResult.Result;

            if (pars.CompanyStorage != null)
            {
                var workingStorageSetResult = worker.SetWorkingStorage(pars.CompanyStorage);
                if (workingStorageSetResult.LocalizedError != null) return workingStorageSetResult.LocalizedError;
            }

            if (pars.Recipe != null)
            {
                var recipeSetResult = worker.SetRecipe(pars.Recipe);
                if (recipeSetResult.LocalizedError != null) return recipeSetResult.LocalizedError;
            }

            return worker;
        }

        public EmptyGameResult StartWorking()
        {
            if (IsWorkingNow)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The work is already in progress",
                    RussianMessage = "Работа уже идёт"
                };
            }

            if (WorkingStorage == null)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Working storage needs to be set before starting to work",
                    RussianMessage = "Для начала необходимо установить рабочее хранилище"
                };
            }
            if (Recipe == null)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The recipe needs to be set before starting to work",
                    RussianMessage = "Для начала необходимо установить рецепт"
                };
            }

            if (!WorkingStorage.HasIngredients(Recipe.Ingredients))
            {
                return new EmptyGameResult
                {
                    LocalizedError = new LocalizedError
                    {
                        ErrorSeverity = ErrorSeverity.Normal,
                        EnglishMessage = "The storage does not have all the ingredients needed.",
                        RussianMessage = "В хранилище нет всех необходимых ингридиентов."
                    }
                };
            }

            WorkingStorage.Remove(Recipe.Ingredients);

            IsWorkingNow = true;

            return new EmptyGameResult();
        }

        public EmptyGameResult ProduceItem()
        {
            if (WorkingStorage == null)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Working storage needs to be set before starting to work",
                    RussianMessage = "Для начала необходимо установить рабочее хранилище"
                };
            }
            if (Recipe == null)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The recipe needs to be set before starting to work",
                    RussianMessage = "Для начала необходимо установить рецепт"
                };
            }

            var additionResult = WorkingStorage.Add(Recipe.ResultItem, Recipe.ResultItemQuantity);
            if (additionResult.LocalizedError != null) return additionResult.LocalizedError;

            return new EmptyGameResult();
        }

        public EmptyGameResult StopWorking()
        {
            IsWorkingNow = false;

            return new EmptyGameResult();
        }

        public EmptyGameResult SetWorkingStorage(CompanyStorage storage)
        {
            if (IsWorkingNow)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Cannot set the storage, as the work is currently being done",
                    RussianMessage = "Нельзя изменить хранилище, так как в настоящее время идет работа"
                };
            }

            if (storage.Type != CompanyStorageType.General)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Can't set working storage of this type",
                    RussianMessage = "Нельзя установить рабочее хранилище этого типа"
                };
            }

            WorkingStorage = storage;

            return new EmptyGameResult();
        }

        public EmptyGameResult SetRecipe(Recipe recipe)
        {
            if (IsWorkingNow)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Cannot set the recipe, as the work is currently being done",
                    RussianMessage = "Нельзя изменить рецепт, так как в настоящее время идет работа"
                };
            }

            Recipe = recipe;

            return new EmptyGameResult();
        }

        public EmptyGameResult UpgradeSpeedMultiplier(decimal step, GameSettings gameSettings)
        {
            if (SpeedMultiplier - step >= gameSettings.Business.Company.Worker.MaxMultiplier)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Working speed is already at the maximum",
                    RussianMessage = "Скорость работы уже на максимуме"
                };
            }

            if (IsWorkingNow)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Can't upgrade, as the work is currently being done",
                    RussianMessage = "Нельзя улучшить, так как ведётся работа. " +
                                     "Остановите работу или подождите, пока она завершится перед тем, как улучшить"
                };
            }

            SpeedMultiplier += step;

            return new EmptyGameResult();
        }


        private static GameResult<CompanyWorker> CreateBase(CompanyWorkerCreationParams pars, GameSettings gameSettings)
        {
            var worker = new CompanyWorker
            {
                Company = pars.Company,

                SpeedMultiplier = gameSettings.Business.Company.Worker.DefaultMultiplier,

                IsWorkingNow = false
            };

            return worker;
        }
        #endregion
    }
}
