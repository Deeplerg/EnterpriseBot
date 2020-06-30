using EnterpriseBot.Api.Game.Storages;
using EnterpriseBot.Api.Models.ModelCreationParams.Business;
using EnterpriseBot.Api.Models.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.Settings.BusinessSettings.Company;
using Hangfire.Server;
using EnterpriseBot.Api.Game.Crafting;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

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

        public int ItemsAmountMadeThisWeek { get; protected set; }

        public decimal SpeedMultiplier { get; protected set; }

        [JsonIgnore]
        public string ProduceItemJobId { get; set; }
        [JsonIgnore]
        public string StopWorkingJobId { get; set; }

        public int LeadTimeInSeconds 
        {
            get => (int)Math.Floor(Recipe.LeadTimeInSeconds / SpeedMultiplier);
        }
        #endregion

        #region actions
        public static GameResult<CompanyWorker> Create(CompanyWorkerCreationParams pars,
            CompanyWorkerSettings workerSettings)
        {
            var workerCreationResult = CreateBase(pars, pars.Company, workerSettings);
            if (workerCreationResult.LocalizedError != null) return workerCreationResult.LocalizedError;

            var worker = workerCreationResult.Result;

            var workingStorageSetResult = worker.SetWorkingStorage(pars.Storage);
            if (workingStorageSetResult.LocalizedError != null) return workingStorageSetResult.LocalizedError;

            var recipeSetResult = worker.SetRecipe(pars.Recipe);
            if (recipeSetResult.LocalizedError != null) return recipeSetResult.LocalizedError;

            return worker;
        }

        public static GameResult<CompanyWorker> Create(CompanyWorkerCreationParams pars,
            Company company, CompanyWorkerSettings workerSettings)
        {
            return CreateBase(pars, company, workerSettings);
        }

        public EmptyGameResult StartWorking()
        {
            if(IsWorkingNow)
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

            ItemsAmountMadeThisWeek += Recipe.ResultItemQuantity;

            return new EmptyGameResult();
        }

        public EmptyGameResult StopWorking()
        {
            IsWorkingNow = false;

            return new EmptyGameResult();
        }

        public EmptyGameResult SetWorkingStorage(CompanyStorage storage)
        {
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
                    EnglishMessage = "Cannot set recipe, as the work is currently being done",
                    RussianMessage = "Нельзя изменить рецепт, так как в настоящее время идет работа"
                };
            }

            Recipe = recipe;

            return new EmptyGameResult();
        }

        public EmptyGameResult ResetItemsAmountMadeThisWeek()
        {
            ItemsAmountMadeThisWeek = 0;

            return new EmptyGameResult();
        }

        public EmptyGameResult UpgradeSpeedMultiplier(decimal step, CompanyWorkerSettings workerSettings)
        {
            if(SpeedMultiplier - step >= workerSettings.MaxMultiplier)
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


        private static GameResult<CompanyWorker> CreateBase(CompanyWorkerCreationParams pars,
            Company company, CompanyWorkerSettings workerSettings)
        {
            var worker = new CompanyWorker
            {
                Company = company,

                SpeedMultiplier = workerSettings.DefaultMultiplier,

                ItemsAmountMadeThisWeek = 0,
                IsWorkingNow = false
            };

            return worker;
        }
        #endregion
    }
}
