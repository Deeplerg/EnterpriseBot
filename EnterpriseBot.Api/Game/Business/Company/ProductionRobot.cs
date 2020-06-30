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
        public int ItemsAmountMadeThisWeek { get => Worker.ItemsAmountMadeThisWeek; }
        public decimal SpeedMultiplier { get => Worker.SpeedMultiplier; }


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

        public static GameResult<ProductionRobot> Create(ProductionRobotCreationParams pars,
            UserInputRequirements inputRequirements, CompanyWorkerSettings workerSettings)
        {
            var robotCreationResult = CreateBase(pars, pars.Company, inputRequirements, workerSettings);
            if (robotCreationResult.LocalizedError != null) return robotCreationResult.LocalizedError;

            var robot = robotCreationResult.Result;

            if (pars.Storage != null || pars.Recipe != null)
            {
                if (pars.Recipe != null && !pars.Recipe.CanBeDoneBy.HasFlag(RecipeCanBeDoneBy.Robot))
                {
                    return recipeCantBeDoneByRobotError;
                }

                robot.Worker = CompanyWorker.Create(new CompanyWorkerCreationParams
                {
                    Company = robot.Company,
                    Recipe = pars.Recipe,
                    Storage = pars.Storage
                }, workerSettings);
            }

            return robot;
        }

        public static EmptyGameResult Buy(Company company, CompanyFeaturesPricesSettings pricesSettings, DonationSettings donationSettings)
        {
            return company.ReduceBusinessCoins(pricesSettings.NewRobotSetup, donationSettings);
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

        public EmptyGameResult ResetItemsAmountMadeThisWeek()
        {
            return Worker.ResetItemsAmountMadeThisWeek();
        }

        public EmptyGameResult Upgrade(CompanyFeaturesPricesSettings pricesSettings, CompanyWorkerSettings workerSettings)
        {
            decimal step = pricesSettings.WorkerModifierUpgrade.Step;
            decimal price = pricesSettings.WorkerModifierUpgrade.Price;

            var buyingUpgradeResult = Company.Purse.Reduce(price, Currency.BusinessCoins);
            if (buyingUpgradeResult.LocalizedError != null) return buyingUpgradeResult.LocalizedError;

            return Worker.UpgradeSpeedMultiplier(step, workerSettings);
        }


        private static GameResult<ProductionRobot> CreateBase(ProductionRobotCreationParams pars,
            Company company, UserInputRequirements inputRequirements, CompanyWorkerSettings workerSettings)
        {
            if (!UserInputUtils.CheckName(pars.Name))
            {
                var req = inputRequirements;

                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,

                    EnglishMessage = string.Format(req.Name.English,
                                                   Constants.NameMaxLength),
                    RussianMessage = string.Format(req.Name.Russian,
                                                   Constants.NameMaxLength)
                };
            }

            return new ProductionRobot
            {
                Name = pars.Name,
                Company = company
            };
        }
    }
}
