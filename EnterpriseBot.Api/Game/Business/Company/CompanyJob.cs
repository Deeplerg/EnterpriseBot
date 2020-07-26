using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Game.Essences;
using EnterpriseBot.Api.Game.Localization;
using EnterpriseBot.Api.Game.Storages;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.ModelCreationParams.Business;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings;
using EnterpriseBot.Api.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static EnterpriseBot.Api.Utils.UserInputUtils;

namespace EnterpriseBot.Api.Game.Business.Company
{
    public class CompanyJob
    {
        protected CompanyJob() { }

        #region model
        public long Id { get; protected set; }

        public virtual LocalizedString Name { get; protected set; }
        public virtual LocalizedString Description { get; protected set; }

        public virtual Player Employee { get; protected set; }

        public virtual Company Company { get; protected set; }

        public CompanyJobPermissions Permissions { get; protected set; }

        public decimal Salary { get; protected set; }

        public virtual IReadOnlyCollection<CompanyJobApplication> Applications
        {
            get => new ReadOnlyCollection<CompanyJobApplication>(applications);
            set => applications = value.ToList();
        }

        private List<CompanyJobApplication> applications = new List<CompanyJobApplication>();


        public bool IsOccupied { get => Employee != null; }
        public Recipe Recipe { get => Worker.Recipe; }
        public CompanyStorage WorkingStorage { get => Worker.WorkingStorage; }
        public bool IsWorkingNow { get => Worker.IsWorkingNow; }

        [JsonIgnore]
        public string ProduceItemAndStopJobId
        {
            get => Worker.ProduceItemAndStopJobId;
            set => Worker.ProduceItemAndStopJobId = value;
        }

        [JsonIgnore]
        public string PaySalaryJobId { get; set; }

        [JsonIgnore]
        public long CompanyWorkerId
        {
            get => Worker.Id;
        }

        [JsonIgnore]
        protected virtual CompanyWorker Worker { get; set; }

        #region errors
        private static readonly LocalizedError recipeCantBeDoneByPlayerError = new LocalizedError
        {
            ErrorSeverity = ErrorSeverity.Normal,
            EnglishMessage = "The recipe can't be done by player",
            RussianMessage = "Рецепт не может быть выполнен игроком"
        };
        private static readonly LocalizedError cantAllowProduceItemsError = new LocalizedError
        {
            ErrorSeverity = ErrorSeverity.Normal,
            EnglishMessage = "Can't allow produce items, as working storage and/or recipe are not specified",
            RussianMessage = "Нельзя разрешить производить предметы, так как рабочее хранилище и/или рецепт не указаны"
        };
        #endregion
        #endregion

        #region actions
        public static GameResult<CompanyJob> Create(CompanyJobCreationParams pars, GameSettings gameSettings, Player invoker)
        {
            if (!invoker.HasPermission(CompanyJobPermissions.CreateJob, pars.Company))
            {
                return Errors.DoesNotHavePermission();
            }

            var jobCreationResult = CreateBase(pars, gameSettings);
            if (jobCreationResult.LocalizedError != null) return jobCreationResult.LocalizedError;

            CompanyJob job = jobCreationResult;

            //if (pars.Permissions.HasFlag(CompanyJobPermissions.ProduceItems))
            //{
            //    if(pars.Recipe == null || pars.CompanyStorage == null)
            //    {
            //        return cantAllowProduceItemsError;
            //    }

            //    if (!pars.Recipe.CanBeDoneBy.HasFlag(RecipeCanBeDoneBy.Player))
            //    {
            //        return recipeCantBeDoneByPlayerError;
            //    }

            //    var workerCreationResult = CompanyWorker.Create(new CompanyWorkerCreationParams
            //    {
            //        Company = pars.Company,
            //        Recipe = pars.Recipe,
            //        Storage = pars.CompanyStorage
            //    }, gameSettings);
            //    if (workerCreationResult.LocalizedError != null) return workerCreationResult.LocalizedError;


            //    job.Worker = workerCreationResult;
            //}

            var workerCreationResult = CompanyWorker.Create(new CompanyWorkerCreationParams
            {
                Company = pars.Company,
                Recipe = pars.Recipe,
                CompanyStorage = pars.CompanyStorage
            }, gameSettings);
            if (workerCreationResult.LocalizedError != null) return workerCreationResult.LocalizedError;


            job.Worker = workerCreationResult;

            return job;
        }

        public GameResult<StringLocalization> SetDescription(string newDescription, LocalizationLanguage language, GameSettings gameSettings, Player invoker)
        {
            if (!invoker.HasPermission(CompanyJobPermissions.ChangeJobParameters, Company))
            {
                return Errors.DoesNotHavePermission();
            }
            if (ThisJobHasPermission(CompanyJobPermissions.ChangeJobParameters))
            {
                return Errors.DoesNotHavePermission();
            }

            if (!CheckDescription(newDescription))
            {
                return Errors.IncorrectDescriptionInput(gameSettings.Localization.UserInputRequirements);
            }

            return Description.Edit(newDescription, language);
        }

        public EmptyGameResult StartWorking()
        {
            if (!ThisJobHasPermission(CompanyJobPermissions.ProduceItems))
            {
                return Errors.DoesNotHavePermission();
            }

            var workingJob = Employee.CompanyJobs.FirstOrDefault(j => j.Worker.IsWorkingNow);
            if (workingJob != null)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = $"You are already producing an item in company '{workingJob.Name}'",
                    RussianMessage = $"Ты уже создаёшь предмет в компании '{workingJob.Name}'"
                };
            }

            if (!Recipe.CanBeDoneBy.HasFlag(RecipeCanBeDoneBy.Player))
            {
                return recipeCantBeDoneByPlayerError;
            }

            return Worker.StartWorking();
        }

        public EmptyGameResult ProduceItem()
        {
            if (!ThisJobHasPermission(CompanyJobPermissions.ProduceItems))
            {
                return Errors.DoesNotHavePermission();
            }

            return Worker.ProduceItem();
        }

        public EmptyGameResult StopWorking()
        {
            return Worker.StopWorking();
        }

        public EmptyGameResult SetWorkingStorage(CompanyStorage storage, Player invoker)
        {
            if (!HasInvokerPermissionToChangeJobParameters(invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            return Worker.SetWorkingStorage(storage);
        }

        public EmptyGameResult SetRecipe(Recipe recipe, Player invoker)
        {
            if (!HasInvokerPermissionToChangeJobParameters(invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            if (!ThisJobHasPermission(CompanyJobPermissions.ProduceItems))
            {
                return new EmptyGameResult
                {
                    LocalizedError = new LocalizedError
                    {
                        ErrorSeverity = ErrorSeverity.Normal,
                        EnglishMessage = "This worker can't produce items",
                        RussianMessage = "Этот работник не может производить предметы"
                    }
                };
            }

            if (!Recipe.CanBeDoneBy.HasFlag(RecipeCanBeDoneBy.Player))
            {
                return recipeCantBeDoneByPlayerError;
            }

            return Worker.SetRecipe(recipe);
        }

        public GameResult<CompanyJobApplication> Apply(Player applicant, string resume, GameSettings gameSettings)
        {
            return CompanyJobApplication.Create(new CompanyJobApplicationCreationParams
            {
                Job = this,
                Applicant = applicant,
                Resume = resume
            }, gameSettings);
        }

        public EmptyGameResult Hire(CompanyJobApplication application, Player invoker)
        {
            if (!invoker.HasPermission(CompanyJobPermissions.Hire, Company))
            {
                return Errors.DoesNotHavePermission();
            }

            if (IsOccupied)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "This job is already occupied",
                    RussianMessage = "Это рабочее место уже занято"
                };
            }

            if (application.Job != this)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Critical,
                    EnglishMessage = $"Job {(application.Job != null ? application.Job.Id.ToString() : "[is null]")} " +
                                     $"in application {application.Id} does not match hiring job {Id}",
                    RussianMessage = $"Работа {(application.Job != null ? application.Job.Id.ToString() : "[является null]")} " +
                                     $"в заявке {application.Id} не совпадает с нанимаемой работой {Id}"
                };
            }

            Employee = application.Applicant;

            return new EmptyGameResult();
        }

        public EmptyGameResult Fire(Player invoker)
        {
            if (invoker != null)
            {
                if (!invoker.HasPermission(CompanyJobPermissions.Fire, Company))
                {
                    return Errors.DoesNotHavePermission();
                }
                if (ThisJobHasPermission(CompanyJobPermissions.Fire))
                {
                    return Errors.DoesNotHavePermission();
                }
            }

            var stopWorkingResult = StopWorking();
            if (stopWorkingResult.LocalizedError != null) return stopWorkingResult.LocalizedError;

            Employee = null;

            return new EmptyGameResult();
        }

        public EmptyGameResult PaySalary()
        {
            var reduceResult = Company.Purse.Reduce(Salary, Currency.Units);
            if (reduceResult.LocalizedError != null) return reduceResult.LocalizedError;

            var addResult = Employee.Purse.Add(Salary, Currency.Units);
            if (addResult.LocalizedError != null) return addResult.LocalizedError;

            return new EmptyGameResult();
        }

        public EmptyGameResult AddPermissions(CompanyJobPermissions permissions, Player invoker)
        {
            if (!HasInvokerPermissionToChangeJobParameters(invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            if (permissions.HasFlag(CompanyJobPermissions.ProduceItems)
                && (Worker == null || Worker.Recipe == null || Worker.WorkingStorage == null))
            {
                return cantAllowProduceItemsError;
            }

            Permissions |= permissions;

            return new EmptyGameResult();
        }

        public EmptyGameResult RemovePermissions(CompanyJobPermissions permissions, Player invoker)
        {
            if (!HasInvokerPermissionToChangeJobParameters(invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            Permissions ^= permissions;
            if (permissions.HasFlag(CompanyJobPermissions.ProduceItems))
            {
                StopWorking();
            }

            return new EmptyGameResult();
        }

        public EmptyGameResult SetPermissions(CompanyJobPermissions permissions, Player invoker)
        {
            if (!HasInvokerPermissionToChangeJobParameters(invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            if (permissions.HasFlag(CompanyJobPermissions.ProduceItems)
                && (Worker == null || Worker.Recipe == null || Worker.WorkingStorage == null))
            {
                return cantAllowProduceItemsError;
            }

            if (!Permissions.HasFlag(CompanyJobPermissions.ProduceItems)
            && permissions.HasFlag(CompanyJobPermissions.ProduceItems))
            {
                StopWorking();
            }

            Permissions = permissions;

            return new EmptyGameResult();
        }

        public GameResult<decimal> SetSalary(decimal newSalary, GameSettings gameSettings, Player invoker)
        {
            var jobSettings = gameSettings.Business.Company.Job;

            if (!invoker.HasPermission(CompanyJobPermissions.ChangeJobParameters, Company))
            {
                return Errors.DoesNotHavePermission();
            }

            if (newSalary < jobSettings.MinSalary)
            {
                return SalaryBelowMinimumError(jobSettings.MinSalary);
            }

            Salary = newSalary;

            return Salary;
        }

        //for current job checking
        public bool ThisJobHasPermission(CompanyJobPermissions permission)
        {
            return this.Permissions.HasFlag(permission) || this.Permissions.HasFlag(CompanyJobPermissions.GeneralManager);
        }


        private bool HasInvokerPermissionToChangeJobParameters(Player invoker)
        {
            if (!invoker.HasPermission(CompanyJobPermissions.ChangeJobParameters, Company))
            {
                return false;
            }
            if (ThisJobHasPermission(CompanyJobPermissions.ChangeJobParameters))
            {
                return false;
            }

            return true;
        }


        private static GameResult<CompanyJob> CreateBase(CompanyJobCreationParams pars, GameSettings gameSettings)
        {
            var req = gameSettings.Localization.UserInputRequirements;
            var jobSettings = gameSettings.Business.Company.Job;

            foreach (var str in pars.Name.Localizations)
            {
                if (!CheckName(str.Text))
                {
                    return Errors.IncorrectNameInput(req);
                }
            }

            foreach (var str in pars.Description.Localizations)
            {
                if (!CheckDescription(str.Text))
                {
                    return Errors.IncorrectDescriptionInput(req);
                }
            }

            if (pars.Salary < jobSettings.MinSalary)
            {
                return SalaryBelowMinimumError(jobSettings.MinSalary);
            }

            return new CompanyJob
            {
                Name = pars.Name,
                Description = pars.Description,

                Company = pars.Company,

                Permissions = pars.Permissions,
            };
        }

        private static LocalizedError SalaryBelowMinimumError(decimal minimum)
        {
            return new LocalizedError
            {
                ErrorSeverity = ErrorSeverity.Normal,
                EnglishMessage = $"The salary can't be below the {minimum}u minimum",
                RussianMessage = $"Зарплата не может быть ниже минимума в {minimum}u"
            };
        }
        #endregion
    }
}
