using EnterpriseBot.Api.Game.Essences;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.ModelCreationParams.Business;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EnterpriseBot.Api.Utils.UserInputUtils;
using static EnterpriseBot.Api.Utils.Constants;
using EnterpriseBot.Api.Game.Storages;
using System.Text;
using EnterpriseBot.Api.Models.Settings.BusinessSettings.Company;
using Hangfire.Storage.Monitoring;
using EnterpriseBot.Api.Game.Crafting;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace EnterpriseBot.Api.Game.Business.Company
{
    public class CompanyJob
    {
        protected CompanyJob() { }

        #region model
        public long Id { get; protected set; }

        public string Name { get; protected set; }
        public string Description { get; protected set; }

        public virtual Player Employee { get; protected set; }

        public virtual Company Company { get; protected set; }

        public CompanyJobPermissions Permissions { get; protected set; }


        protected virtual CompanyWorker Worker { get; set; }


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
        public int ItemsAmountMadeThisWeek { get => Worker.ItemsAmountMadeThisWeek; }

        [JsonIgnore]
        public string ProduceItemJobId 
        {
            get => Worker.ProduceItemJobId;
            set => Worker.ProduceItemJobId = value;
        }

        [JsonIgnore]
        public string StopWorkingJobId
        {
            get => Worker.StopWorkingJobId;
            set => Worker.StopWorkingJobId = value;
        }
        #endregion

        #region actions
        /// <summary>
        /// Creates <see cref="CompanyJob"/> instance. Used when the job <b>does NOT</b> have <see cref="CompanyJobPermissions.ProduceItems"/>
        /// </summary>
        /// <param name="pars"></param>
        /// <param name="inputRequirements"></param>
        /// <param name="invoker"></param>
        /// <returns></returns>
        public static GameResult<CompanyJob> Create(CompanyJobCreationParams pars, 
            UserInputRequirements inputRequirements, Player invoker)
        {
            if(!invoker.HasPermission(CompanyJobPermissions.CreateJob, pars.Company))
            {
                return Errors.DoesNotHavePermission();
            }
            if(pars.Permissions.HasFlag(CompanyJobPermissions.ProduceItems))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Can't allow produce items, as working storage and/or recipe are not specified",
                    RussianMessage = "Нельзя разрешить производить предметы, так как рабочее хранилище и/или рецепт не указаны"
                };
            }

            return CreateBase(pars, inputRequirements);
        }

        /// <summary>
        /// Creates <see cref="CompanyJob"/> instance. Used when the job <b>does</b> have <see cref="CompanyJobPermissions.ProduceItems"/>
        /// </summary>
        /// <param name="pars"></param>
        /// <param name="inputRequirements"></param>
        /// <param name="workerSettings"></param>
        /// <param name="invoker"></param>
        /// <returns></returns>
        public static GameResult<CompanyJob> Create(CompanyJobCreationParams pars,
            UserInputRequirements inputRequirements, CompanyWorkerSettings workerSettings, Player invoker)
        {
            if (!invoker.HasPermission(CompanyJobPermissions.CreateJob, pars.Company))
            {
                return Errors.DoesNotHavePermission();
            }

            var jobCreationResult = CreateBase(pars, inputRequirements);
            if (jobCreationResult.LocalizedError != null) return jobCreationResult.LocalizedError;

            CompanyJob job = jobCreationResult;

            if (!pars.Recipe.CanBeDoneBy.HasFlag(RecipeCanBeDoneBy.Player))
            {
                return RecipeCantBeDoneByPlayerError();
            }

            var workerCreationResult = CompanyWorker.Create(new CompanyWorkerCreationParams
            {
                Company = pars.Company,
                Recipe = pars.Recipe,
                Storage = pars.Storage
            }, workerSettings);
            if (workerCreationResult.LocalizedError != null) return workerCreationResult.LocalizedError;


            job.Worker = workerCreationResult;

            return job;
        }

        public GameResult<string> SetDescription(string newDescription, UserInputRequirements inputRequirements, Player invoker)
        {
            if(!invoker.HasPermission(CompanyJobPermissions.ChangeJobParameters, Company))
            {
                return Errors.DoesNotHavePermission();
            }
            if(ThisJobHasPermission(CompanyJobPermissions.ChangeJobParameters))
            {
                return Errors.DoesNotHavePermission();
            }

            if (!CheckDescription(newDescription))
            {
                var req = inputRequirements;

                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = string.Format(req.Description.English,
                                                   DescriptionMaxLength),
                    RussianMessage = string.Format(req.Description.Russian,
                                                   DescriptionMaxLength)
                };
            }

            Description = newDescription;

            return Description;
        }

        public EmptyGameResult StartWorking()
        {
            if(!ThisJobHasPermission(CompanyJobPermissions.ProduceItems))
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

            if(!Recipe.CanBeDoneBy.HasFlag(RecipeCanBeDoneBy.Player))
            {
                return RecipeCantBeDoneByPlayerError();
            }

            return Worker.StartWorking();
        }

        public EmptyGameResult ProduceItem()
        {
            if(!ThisJobHasPermission(CompanyJobPermissions.ProduceItems))
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
                return RecipeCantBeDoneByPlayerError();
            }

            return Worker.SetRecipe(recipe);
        }

        public GameResult<CompanyJobApplication> Apply(Player applicant, string resume, UserInputRequirements inputReq)
        {
            return CompanyJobApplication.Create(new CompanyJobApplicationCreationParams
            {
                Job = this,
                Applicant = applicant,
                Resume = resume
            }, inputReq);
        }

        public EmptyGameResult AddPermissions(CompanyJobPermissions permissions, Player invoker)
        {
            if(!HasInvokerPermissionToChangeJobParameters(invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            if(permissions.HasFlag(CompanyJobPermissions.ProduceItems) 
                && (Worker == null || Worker.Recipe == null || Worker.WorkingStorage == null))
            {
                return CantAllowProduceItemsError();
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

            return new EmptyGameResult();
        }

        public EmptyGameResult SetGamePermissions(CompanyJobPermissions permissions, Player invoker)
        {
            if (!HasInvokerPermissionToChangeJobParameters(invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            if (permissions.HasFlag(CompanyJobPermissions.ProduceItems)
                && (Worker == null || Worker.Recipe == null || Worker.WorkingStorage == null))
            {
                return CantAllowProduceItemsError();
            }

            Permissions = permissions;

            return new EmptyGameResult();
        }

        public EmptyGameResult ResetItemsAmountMadeThisWeek()
        {
            return Worker.ResetItemsAmountMadeThisWeek();
        }

        //for current job checking
        public bool ThisJobHasPermission(CompanyJobPermissions permission)
        {
            return this.Permissions.HasFlag(permission);
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

        private LocalizedError CantAllowProduceItemsError()
        {
            return new LocalizedError
            {
                ErrorSeverity = ErrorSeverity.Normal,
                EnglishMessage = "Can't allow produce items, as working storage and/or recipe are not specified",
                RussianMessage = "Нельзя разрешить производить предметы, так как рабочее хранилище и/или рецепт не указаны"
            };
        }


        private static GameResult<CompanyJob> CreateBase(CompanyJobCreationParams pars, 
            UserInputRequirements inputRequirements)
        {
            var req = inputRequirements;

            if (!CheckName(pars.Name))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = string.Format(req.Name.English,
                                                   NameMaxLength),
                    RussianMessage = string.Format(req.Name.Russian,
                                                   NameMaxLength)
                };
            }
            if (!CheckDescription(pars.Name))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = string.Format(req.Description.English,
                                                   DescriptionMaxLength),
                    RussianMessage = string.Format(req.Description.Russian,
                                                   DescriptionMaxLength)
                };
            }

            return new CompanyJob
            {
                Name = pars.Name,
                Description = pars.Description,

                Company = pars.Company,

                Permissions = pars.Permissions,
            };
        }

        private static LocalizedError RecipeCantBeDoneByPlayerError()
        {
            return new LocalizedError
            {
                ErrorSeverity = ErrorSeverity.Normal,
                EnglishMessage = "The recipe can't be done by player",
                RussianMessage = "Рецепт не может быть выполнен игроком"
            };
        }
        #endregion
    }
}
