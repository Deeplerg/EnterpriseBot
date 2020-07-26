using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Attributes;
using EnterpriseBot.Api.Game.Business.Company;
using EnterpriseBot.Api.Game.Localization;
using EnterpriseBot.Api.Models.ApiCreationParams.Business;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.Contexts;
using EnterpriseBot.Api.Models.ModelCreationParams.Business;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using EnterpriseBot.BackgroundJobs.Jobs;
using EnterpriseBot.BackgroundJobs.Params;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Areas.Business.Company.Controllers
{
    [Area(nameof(Business))]
    [SubArea(nameof(Business.Company))]
    public class CompanyJobController : Controller,
                                        IGameController<CompanyJob, CompanyJobApiCreationParams>
    {
        private readonly ApplicationContext ctx;
        private readonly ILogger<CompanyJobController> logger;

        private readonly GameSettings gameSettings;

        private readonly LocalizationSettings localization;
        private readonly LocalizationSetting modelLocalization;

        public CompanyJobController(ApplicationContext dbContext,
                                    ILogger<CompanyJobController> logger,
                                    IOptionsSnapshot<GameSettings> gameOptionsAccessor)
        {
            this.ctx = dbContext;
            this.logger = logger;

            this.gameSettings = gameOptionsAccessor.Value;

            this.localization = this.gameSettings.Localization;
            this.modelLocalization = this.localization.Business.Company.CompanyJob;
        }

        ///<inheritdoc/>
        public async Task<GameResult<CompanyJob>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            var model = await ctx.CompanyJobs.FindAsync(id);

            return model;
        }

        public async Task<GameResult<CompanyJob>> Create([FromBody] CompanyJobApiCreationParams pars)
        {
            var name = await ctx.LocalizedStrings.FindAsync(pars.NameLocalizedStringId);
            if (name == null) return Errors.DoesNotExist(pars.NameLocalizedStringId, localization.Localization.LocalizedString);

            var description = await ctx.LocalizedStrings.FindAsync(pars.DescriptionLocalizedStringId);
            if (description == null) return Errors.DoesNotExist(pars.DescriptionLocalizedStringId, localization.Localization.LocalizedString);

            var company = await ctx.Companies.FindAsync(pars.CompanyId);
            if (company == null) return Errors.DoesNotExist(pars.CompanyId, localization.Business.Company.Company);

            var recipe = await ctx.Recipes.FindAsync(pars.RecipeId);

            var companyStorage = await ctx.CompanyStorages.FindAsync(pars.CompanyStorageId);

            var invoker = await ctx.Players.FindAsync(pars.InvokerPlayerId);
            if (invoker == null) return Errors.DoesNotExist(pars.InvokerPlayerId, localization.Essences.Player);

            var creationResult = CompanyJob.Create(new CompanyJobCreationParams
            {
                Name = name,
                Description = description,
                Company = company,
                Recipe = recipe,
                CompanyStorage = companyStorage,

                Salary = pars.Salary,
                Permissions = pars.Permissions
            }, gameSettings, invoker);
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.CompanyJobs.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }


        public async Task<GameResult<StringLocalization>> SetDescription([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                newDescription = default(string),
                language = default(LocalizationLanguage),
                invokerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var companyJob = await ctx.CompanyJobs.FindAsync(d.modelId);
            if (companyJob == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);

            var actionResult = companyJob.SetDescription(d.newDescription, d.language, gameSettings, invoker);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<EmptyGameResult> StartWorking([FromBody] string json, [FromServices] IBackgroundJobScheduler jobs)
        {
            var pars = new
            {
                modelId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var companyJob = await ctx.CompanyJobs.FindAsync(d.modelId);
            if (companyJob == null) return Errors.DoesNotExist(d.modelId, modelLocalization);


            var actionResult = companyJob.StartWorking();
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            ScheduleProduceItemAndStopJob(companyJob, jobs);

            await ctx.SaveChangesAsync();

            return new EmptyGameResult();
        }

        public async Task<EmptyGameResult> ProduceItem([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var companyJob = await ctx.CompanyJobs.FindAsync(d.modelId);
            if (companyJob == null) return Errors.DoesNotExist(d.modelId, modelLocalization);


            var actionResult = companyJob.ProduceItem();
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return new EmptyGameResult();
        }

        public async Task<EmptyGameResult> StopWorking([FromBody] string json, [FromServices] IBackgroundJobScheduler jobs)
        {
            var pars = new
            {
                modelId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var companyJob = await ctx.CompanyJobs.FindAsync(d.modelId);
            if (companyJob == null) return Errors.DoesNotExist(d.modelId, modelLocalization);


            var actionResult = companyJob.StopWorking();
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            jobs.Remove(companyJob.ProduceItemAndStopJobId);

            await ctx.SaveChangesAsync();

            return new EmptyGameResult();
        }

        public async Task<EmptyGameResult> SetWorkingStorage([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                companyStorageId = default(long),
                invokerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var companyJob = await ctx.CompanyJobs.FindAsync(d.modelId);
            if (companyJob == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var companyStorage = await ctx.CompanyStorages.FindAsync(d.companyStorageId);
            if (companyStorage == null) return Errors.DoesNotExist(d.modelId, localization.Storages.CompanyStorage);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);


            var actionResult = companyJob.SetWorkingStorage(companyStorage, invoker);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<EmptyGameResult> SetRecipe([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                recipeId = default(long),
                invokerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var companyJob = await ctx.CompanyJobs.FindAsync(d.modelId);
            if (companyJob == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var recipe = await ctx.Recipes.FindAsync(d.recipeId);
            if (recipe == null) return Errors.DoesNotExist(d.modelId, localization.Crafting.Recipe);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);


            var actionResult = companyJob.SetRecipe(recipe, invoker);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<CompanyJobApplication>> Apply([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                applicantId = default(long),
                resume = default(string)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var companyJob = await ctx.CompanyJobs.FindAsync(d.modelId);
            if (companyJob == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var applicant = await ctx.Players.FindAsync(d.applicantId);
            if (applicant == null) return Errors.DoesNotExist(d.applicantId, localization.Essences.Player);


            var actionResult = companyJob.Apply(applicant, d.resume, gameSettings);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<CompanyJob>> Hire([FromBody] string json, [FromServices] IBackgroundJobScheduler jobs)
        {
            var pars = new
            {
                modelId = default(long),
                applicationId = default(long),
                invokerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var companyJob = await ctx.CompanyJobs.FindAsync(d.modelId);
            if (companyJob == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var jobApplication = await ctx.CompanyJobApplications.FindAsync(d.applicationId);
            if (jobApplication == null) return Errors.DoesNotExist(d.applicationId, localization.Business.Company.CompanyJobApplication);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);


            var actionResult = companyJob.Hire(jobApplication, invoker);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            SchedulePaySalaryJob(companyJob, jobs);

            ctx.Remove(jobApplication);
            await ctx.SaveChangesAsync();

            return companyJob;
        }

        public async Task<GameResult<CompanyJob>> Fire([FromBody] string json, [FromServices] IBackgroundJobScheduler jobs)
        {
            var pars = new
            {
                modelId = default(long),
                invokerId = default(long?)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var companyJob = await ctx.CompanyJobs.FindAsync(d.modelId);
            if (companyJob == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var invoker = await ctx.Players.FindAsync(d.invokerId);


            var actionResult = companyJob.Fire(invoker);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            jobs.Remove(companyJob.PaySalaryJobId);
            jobs.Remove(companyJob.ProduceItemAndStopJobId);

            await ctx.SaveChangesAsync();

            return companyJob;
        }

        public async Task<EmptyGameResult> PaySalary([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var companyJob = await ctx.CompanyJobs.FindAsync(d.modelId);
            if (companyJob == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = companyJob.PaySalary();
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return new EmptyGameResult();
        }

        public async Task<GameResult<CompanyJobPermissions>> AddPermissions([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                newPermissions = default(CompanyJobPermissions),
                invokerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var companyJob = await ctx.CompanyJobs.FindAsync(d.modelId);
            if (companyJob == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);


            var actionResult = companyJob.AddPermissions(d.newPermissions, invoker);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return companyJob.Permissions;
        }

        public async Task<GameResult<CompanyJobPermissions>> RemovePermissions([FromBody] string json, [FromServices] IBackgroundJobScheduler jobs)
        {
            var pars = new
            {
                modelId = default(long),
                permissionsToRemove = default(CompanyJobPermissions),
                invokerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var companyJob = await ctx.CompanyJobs.FindAsync(d.modelId);
            if (companyJob == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);


            var actionResult = companyJob.RemovePermissions(d.permissionsToRemove, invoker);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            if (d.permissionsToRemove.HasFlag(CompanyJobPermissions.ProduceItems))
            {
                jobs.Remove(companyJob.ProduceItemAndStopJobId);
            }

            await ctx.SaveChangesAsync();

            return companyJob.Permissions;
        }

        public async Task<GameResult<CompanyJobPermissions>> SetPermissions([FromBody] string json, [FromServices] IBackgroundJobScheduler jobs)
        {
            var pars = new
            {
                modelId = default(long),
                newPermissions = default(CompanyJobPermissions),
                invokerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var companyJob = await ctx.CompanyJobs.FindAsync(d.modelId);
            if (companyJob == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);


            bool stopWorkingNeeded;

            if (!companyJob.Permissions.HasFlag(CompanyJobPermissions.ProduceItems)
                && d.newPermissions.HasFlag(CompanyJobPermissions.ProduceItems))
            {
                stopWorkingNeeded = true;
            }
            else
            {
                stopWorkingNeeded = false;
            }

            var actionResult = companyJob.SetPermissions(d.newPermissions, invoker);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            if (stopWorkingNeeded)
            {
                jobs.Remove(companyJob.ProduceItemAndStopJobId);
            }

            await ctx.SaveChangesAsync();

            return companyJob.Permissions;
        }

        public async Task<GameResult<decimal>> SetSalary([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                newSalary = default(decimal),
                invokerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var companyJob = await ctx.CompanyJobs.FindAsync(d.modelId);
            if (companyJob == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);


            var actionResult = companyJob.SetSalary(d.newSalary, gameSettings, invoker);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<bool>> DoesJobHavePermission([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                permission = default(CompanyJobPermissions)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var companyJob = await ctx.CompanyJobs.FindAsync(d.modelId);
            if (companyJob == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = companyJob.ThisJobHasPermission(d.permission);

            return actionResult;
        }

        public async Task<EmptyGameResult> SchedulePaySalary([FromBody] string json, [FromServices] IBackgroundJobScheduler jobs)
        {
            var pars = new
            {
                modelId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var companyJob = await ctx.CompanyJobs.FindAsync(d.modelId);
            if (companyJob == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            SchedulePaySalaryJob(companyJob, jobs);

            await ctx.SaveChangesAsync();

            return new EmptyGameResult();
        }


        private void ScheduleProduceItemAndStopJob(CompanyJob companyJob, IBackgroundJobScheduler jobs)
        {
            string jobId = jobs.Schedule<ProduceItemAndStopJob, ProduceItemAndStopJobParams>(new ProduceItemAndStopJobParams
            {
                CompanyWorkerId = companyJob.CompanyWorkerId
            }, TimeSpan.FromSeconds(companyJob.Recipe.LeadTimeInSeconds));

            companyJob.ProduceItemAndStopJobId = jobId;
        }

        private void SchedulePaySalaryJob(CompanyJob companyJob, IBackgroundJobScheduler jobs)
        {
            string jobId = jobs.Schedule<PaySalaryJob, PaySalaryJobParams>(new PaySalaryJobParams
            {
                JobId = companyJob.Id
            }, TimeSpan.FromDays(1));

            companyJob.PaySalaryJobId = jobId;
        }
    }
}
