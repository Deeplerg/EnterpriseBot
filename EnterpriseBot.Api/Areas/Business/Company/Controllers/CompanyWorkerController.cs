using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Attributes;
using EnterpriseBot.Api.Game.Business.Company;
using EnterpriseBot.Api.Models.ApiCreationParams.Business;
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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Areas.Business.Company.Controllers
{
    [Area(nameof(Business))]
    [SubArea(nameof(Business.Company))]
    public class CompanyWorkerController : Controller,
                                           IGameController<CompanyWorker, CompanyWorkerApiCreationParams>
    {
        private readonly ApplicationContext ctx;
        private readonly ILogger<CompanyWorkerController> logger;

        private readonly GameSettings gameSettings;

        private readonly LocalizationSettings localization;
        private readonly LocalizationSetting modelLocalization;

        public CompanyWorkerController(ApplicationContext dbContext,
                                       ILogger<CompanyWorkerController> logger,
                                       IOptionsSnapshot<GameSettings> gameOptionsAccessor)
        {
            this.ctx = dbContext;
            this.logger = logger;

            this.gameSettings = gameOptionsAccessor.Value;

            this.localization = this.gameSettings.Localization;
            this.modelLocalization = this.localization.Business.Company.CompanyWorker;
        }

        ///<inheritdoc/>
        public async Task<GameResult<CompanyWorker>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            var model = await ctx.CompanyWorkers.FindAsync(id);

            return model;
        }

        public async Task<GameResult<CompanyWorker>> Create([FromBody] CompanyWorkerApiCreationParams pars)
        {
            var company = await ctx.Companies.FindAsync(pars.CompanyId);
            if (company == null) return Errors.DoesNotExist(pars.CompanyId, localization.Business.Company.Company);

            var recipe = await ctx.Recipes.FindAsync(pars.RecipeId);

            var companyStorage = await ctx.CompanyStorages.FindAsync(pars.CompanyStorageId);

            var creationResult = CompanyWorker.Create(new CompanyWorkerCreationParams
            {
                Company = company,
                Recipe = recipe,
                CompanyStorage = companyStorage
            }, gameSettings);
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.CompanyWorkers.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }

        public async Task<EmptyGameResult> StartWorking([FromBody] string json, [FromServices] IBackgroundJobScheduler jobs)
        {
            var pars = new
            {
                modelId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var companyWorker = await ctx.CompanyWorkers.FindAsync(d.modelId);
            if (companyWorker == null) return Errors.DoesNotExist(d.modelId, modelLocalization);


            var actionResult = companyWorker.StartWorking();
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            // Why would one calling StartWorking on CompanyWorker (explicitly!) want it to stop?
            //ScheduleProduceItemAndStopJob(companyWorker, jobs);

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

            var companyWorker = await ctx.CompanyWorkers.FindAsync(d.modelId);
            if (companyWorker == null) return Errors.DoesNotExist(d.modelId, modelLocalization);


            var actionResult = companyWorker.ProduceItem();
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

            var companyWorker = await ctx.CompanyWorkers.FindAsync(d.modelId);
            if (companyWorker == null) return Errors.DoesNotExist(d.modelId, modelLocalization);


            var actionResult = companyWorker.StopWorking();
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            jobs.Remove(companyWorker.ProduceItemAndStopJobId);

            await ctx.SaveChangesAsync();

            return new EmptyGameResult();
        }

        public async Task<EmptyGameResult> SetWorkingStorage([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                companyStorageId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var companyWorker = await ctx.CompanyWorkers.FindAsync(d.modelId);
            if (companyWorker == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var companyStorage = await ctx.CompanyStorages.FindAsync(d.companyStorageId);
            if (companyStorage == null) return Errors.DoesNotExist(d.modelId, localization.Storages.CompanyStorage);


            var actionResult = companyWorker.SetWorkingStorage(companyStorage);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<EmptyGameResult> SetRecipe([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                recipeId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var companyWorker = await ctx.CompanyWorkers.FindAsync(d.modelId);
            if (companyWorker == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var recipe = await ctx.Recipes.FindAsync(d.recipeId);
            if (recipe == null) return Errors.DoesNotExist(d.modelId, localization.Crafting.Recipe);


            var actionResult = companyWorker.SetRecipe(recipe);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<decimal>> UpgradeSpeedMultiplier([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                step = default(decimal)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var companyWorker = await ctx.CompanyWorkers.FindAsync(d.modelId);
            if (companyWorker == null) return Errors.DoesNotExist(d.modelId, modelLocalization);


            var actionResult = companyWorker.UpgradeSpeedMultiplier(d.step, gameSettings);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return companyWorker.SpeedMultiplier;
        }



        private void ScheduleProduceItemAndStopJob(CompanyWorker companyWorker, IBackgroundJobScheduler jobs)
        {
            string jobId = jobs.Schedule<ProduceItemAndStopJob, ProduceItemAndStopJobParams>(new ProduceItemAndStopJobParams
            {
                CompanyWorkerId = companyWorker.Id
            }, TimeSpan.FromSeconds(companyWorker.Recipe.LeadTimeInSeconds));

            companyWorker.ProduceItemAndStopJobId = jobId;
        }
    }
}