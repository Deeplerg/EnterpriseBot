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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Areas.Business.Company.Controllers
{
    [Area(nameof(Business))]
    [SubArea(nameof(Business.Company))]
    public class ProductionRobotController : Controller,
                                             IGameController<ProductionRobot, ProductionRobotApiCreationParams>
    {
        private readonly ApplicationContext ctx;
        private readonly ILogger<ProductionRobotController> logger;

        private readonly GameSettings gameSettings;

        private readonly LocalizationSettings localization;
        private readonly LocalizationSetting modelLocalization;

        public ProductionRobotController(ApplicationContext dbContext,
                                         ILogger<ProductionRobotController> logger,
                                         IOptionsSnapshot<GameSettings> gameOptionsAccessor)
        {
            this.ctx = dbContext;
            this.logger = logger;

            this.gameSettings = gameOptionsAccessor.Value;

            this.localization = this.gameSettings.Localization;
            this.modelLocalization = this.localization.Business.Company.ProductionRobot;
        }

        ///<inheritdoc/>
        public async Task<GameResult<ProductionRobot>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            var model = await ctx.ProductionRobots.FindAsync(id);

            return model;
        }

        public async Task<GameResult<ProductionRobot>> Create([FromBody] ProductionRobotApiCreationParams pars)
        {
            var company = await ctx.Companies.FindAsync(pars.CompanyId);
            if (company == null) return Errors.DoesNotExist(pars.CompanyId, localization.Business.Company.Company);

            var recipe = await ctx.Recipes.FindAsync(pars.RecipeId);

            var companyStorage = await ctx.CompanyStorages.FindAsync(pars.CompanyStorageId);

            var creationResult = ProductionRobot.Create(new ProductionRobotCreationParams
            {
                Name = pars.Name,
                Company = company,
                Recipe = recipe,
                CompanyStorage = companyStorage
            }, gameSettings);
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.ProductionRobots.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }

        public async Task<EmptyGameResult> Buy([FromBody] string json)
        {
            var pars = new
            {
                companyId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var company = await ctx.Companies.FindAsync(d.companyId);
            if (company == null) return Errors.DoesNotExist(d.companyId, localization.Business.Company.Company);

            var actionResult = ProductionRobot.Buy(company, gameSettings);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            return new EmptyGameResult();
        }


        public async Task<EmptyGameResult> StartWorking([FromBody] string json, [FromServices] IBackgroundJobScheduler jobs)
        {
            var pars = new
            {
                modelId = default(long),
                invokerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var productionRobot = await ctx.ProductionRobots.FindAsync(d.modelId);
            if (productionRobot == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);


            var actionResult = productionRobot.StartWorking(invoker);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            // Why would one calling StartWorking on ProductionRobot (explicitly!) want it to stop?
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

            var productionRobot = await ctx.ProductionRobots.FindAsync(d.modelId);
            if (productionRobot == null) return Errors.DoesNotExist(d.modelId, modelLocalization);


            var actionResult = productionRobot.ProduceItem();
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return new EmptyGameResult();
        }

        public async Task<EmptyGameResult> StopWorking([FromBody] string json, [FromServices] IBackgroundJobScheduler jobs)
        {
            var pars = new
            {
                modelId = default(long),
                invokerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var productionRobot = await ctx.ProductionRobots.FindAsync(d.modelId);
            if (productionRobot == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);


            var actionResult = productionRobot.StopWorking(invoker);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            //jobs.Remove(productionRobot.ProduceItemAndStopJobId);

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

            var productionRobot = await ctx.ProductionRobots.FindAsync(d.modelId);
            if (productionRobot == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var companyStorage = await ctx.CompanyStorages.FindAsync(d.companyStorageId);
            if (companyStorage == null) return Errors.DoesNotExist(d.modelId, localization.Storages.CompanyStorage);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);


            var actionResult = productionRobot.SetWorkingStorage(companyStorage, invoker);
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

            var productionRobot = await ctx.ProductionRobots.FindAsync(d.modelId);
            if (productionRobot == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var recipe = await ctx.Recipes.FindAsync(d.recipeId);
            if (recipe == null) return Errors.DoesNotExist(d.modelId, localization.Crafting.Recipe);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);


            var actionResult = productionRobot.SetRecipe(recipe, invoker);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<EmptyGameResult> Upgrade([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                step = default(decimal),
                invokerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var productionRobot = await ctx.ProductionRobots.FindAsync(d.modelId);
            if (productionRobot == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);


            var actionResult = productionRobot.Upgrade(gameSettings, invoker);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return new EmptyGameResult();
        }



        //private void ScheduleProduceItemAndStopJob(ProductionRobot productionRobot, IBackgroundJobScheduler jobs)
        //{
        //    string jobId = jobs.Schedule<ProduceItemAndStopJob, ProduceItemAndStopJobParams>(new ProduceItemAndStopJobParams
        //    {
        //        CompanyWorkerId = productionRobot.CompanyWorkerId
        //    }, TimeSpan.FromSeconds(productionRobot.Recipe.LeadTimeInSeconds));

        //    productionRobot.ProduceItemAndStopJobId = jobId;
        //}
    }
}
