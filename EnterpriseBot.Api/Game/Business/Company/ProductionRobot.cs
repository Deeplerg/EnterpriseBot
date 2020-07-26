using EnterpriseBot.Api.Game.Storages;
using EnterpriseBot.Api.Models.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnterpriseBot.Api.Models.ModelCreationParams.Business;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using EnterpriseBot.Api.Models.Settings.BusinessSettings.Company;
using EnterpriseBot.Api.Game.Essences;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Game.Money;
using EnterpriseBot.Api.Models.Settings.BusinessPricesSettings.Company;
using System.ComponentModel.DataAnnotations.Schema;
using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Models.Settings.DonationSettings;
using EnterpriseBot.Api.Models.Settings;
using Newtonsoft.Json;
using Hangfire.Server;

namespace EnterpriseBot.Api.Game.Business.Company
{
    public class ProductionRobot
    {
        protected ProductionRobot() { }

        #region model
        public long Id { get; protected set; }
        public string Name { get; protected set; }

        public virtual Company Company { get; protected set; }


        public Recipe Recipe { get => Worker.Recipe; }
        public CompanyStorage WorkingStorage { get => Worker.WorkingStorage; }
        public bool IsWorkingNow { get => Worker.IsWorkingNow; }
        public decimal SpeedMultiplier { get => Worker.SpeedMultiplier; }
        

        [JsonIgnore]
        public long CompanyWorkerId
        {
            get => Worker.Id;
        }

        [JsonIgnore]
        protected virtual CompanyWorker Worker { get; set; }

        #region errors
        private static readonly LocalizedError recipeCantBeDoneByRobotError = new LocalizedError
        {
            ErrorSeverity = ErrorSeverity.Normal,
            EnglishMessage = "The recipe can't be done by a machine",
            RussianMessage = "Рецепт не может быть выполнен станком"
        };
        #endregion
        #endregion

        public static GameResult<ProductionRobot> Create(ProductionRobotCreationParams pars, GameSettings gameSettings, Player invoker = null)
        {
            var req = gameSettings.Localization.UserInputRequirements;
            var workerSettings = gameSettings.Business.Company.Worker;

            if(invoker != null && !invoker.HasPermission(CompanyJobPermissions.BuyRobots, pars.Company))
            {
                return Errors.DoesNotHavePermission();
            }

            var robotCreationResult = CreateBase(pars, gameSettings);
            if (robotCreationResult.LocalizedError != null) return robotCreationResult.LocalizedError;

            var robot = robotCreationResult.Result;

            //if (pars.CompanyStorage != null || pars.Recipe != null)
            //{
            //    if (pars.Recipe != null && !pars.Recipe.CanBeDoneBy.HasFlag(RecipeCanBeDoneBy.Robot))
            //    {
            //        return recipeCantBeDoneByRobotError;
            //    }

            //    var workerCreationResult = CompanyWorker.Create(new CompanyWorkerCreationParams
            //    {
            //        Company = robot.Company,
            //        Recipe = pars.Recipe,
            //        CompanyStorage = pars.CompanyStorage
            //    }, gameSettings);
            //    if (workerCreationResult.LocalizedError != null) return workerCreationResult.LocalizedError;

            //    robot.Worker = workerCreationResult;
            //}

            if (pars.Recipe != null && !pars.Recipe.CanBeDoneBy.HasFlag(RecipeCanBeDoneBy.Robot))
            {
                return recipeCantBeDoneByRobotError;
            }

            var workerCreationResult = CompanyWorker.Create(new CompanyWorkerCreationParams
            {
                Company = robot.Company,
                Recipe = pars.Recipe,
                CompanyStorage = pars.CompanyStorage
            }, gameSettings);
            if (workerCreationResult.LocalizedError != null) return workerCreationResult.LocalizedError;

            robot.Worker = workerCreationResult;

            return robot;
        }

        public static EmptyGameResult Buy(Company company, GameSettings gameSettings)
        {
            decimal price = gameSettings.BusinessPrices.CompanyFeatures.NewRobotSetup;

            return company.ReduceBusinessCoins(price, gameSettings);
        }


        public EmptyGameResult StartWorking(Player invoker)
        {
            if(!invoker.HasPermission(CompanyJobPermissions.ManageRobotTasks, Company))
            {
                return Errors.DoesNotHavePermission();
            }

            if (!Recipe.CanBeDoneBy.HasFlag(RecipeCanBeDoneBy.Robot))
            {
                return recipeCantBeDoneByRobotError;
            }

            return Worker.StartWorking();
        }

        public EmptyGameResult ProduceItem()
        {
            return Worker.ProduceItem();
        }

        public EmptyGameResult StopWorking(Player invoker)
        {
            if (!invoker.HasPermission(CompanyJobPermissions.ManageRobotTasks, Company))
            {
                return Errors.DoesNotHavePermission();
            }

            return Worker.StopWorking();
        }

        public EmptyGameResult SetWorkingStorage(CompanyStorage storage, Player invoker)
        {
            if (!invoker.HasPermission(CompanyJobPermissions.ChangeRobotParameters, Company))
            {
                return Errors.DoesNotHavePermission();
            }

            return Worker.SetWorkingStorage(storage);
        }
        
        public EmptyGameResult SetRecipe(Recipe recipe, Player invoker)
        {
            if(!invoker.HasPermission(CompanyJobPermissions.ManageRobotTasks, Company))
            {
                return Errors.DoesNotHavePermission();
            }

            if (!Recipe.CanBeDoneBy.HasFlag(RecipeCanBeDoneBy.Robot))
            {
                return recipeCantBeDoneByRobotError;
            }

            return Worker.SetRecipe(recipe);
        }

        public EmptyGameResult Upgrade(GameSettings gameSettings, Player invoker)
        {
            if(!invoker.HasPermission(CompanyJobPermissions.UpgradeRobots, Company))
            {
                return Errors.DoesNotHavePermission();
            }

            var prices = gameSettings.BusinessPrices.CompanyFeatures;

            decimal step = prices.WorkerModifierUpgrade.Step;
            decimal price = prices.WorkerModifierUpgrade.Price;

            var reduceResult = Company.Purse.Reduce(price, Currency.BusinessCoins);
            if (reduceResult.LocalizedError != null) return reduceResult.LocalizedError;

            return Worker.UpgradeSpeedMultiplier(step, gameSettings);
        }


        private static GameResult<ProductionRobot> CreateBase(ProductionRobotCreationParams pars, GameSettings gameSettings)
        {
            if (!UserInputUtils.CheckName(pars.Name))
            {
                return Errors.IncorrectNameInput(gameSettings.Localization.UserInputRequirements);
            }

            return new ProductionRobot
            {
                Name = pars.Name,
                Company = pars.Company
            };
        }
    }
}
