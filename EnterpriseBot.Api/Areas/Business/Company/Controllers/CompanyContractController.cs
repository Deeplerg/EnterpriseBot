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
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Areas.Business.Company.Controllers
{
    [Area(nameof(Business))]
    [SubArea(nameof(Business.Company))]
    public class CompanyContractController : Controller,
                                             IGameController<CompanyContract, CompanyContractApiCreationParams>
    {
        private readonly ApplicationContext ctx;
        private readonly ILogger<CompanyContractController> logger;

        private readonly GameSettings gameSettings;

        private readonly LocalizationSettings localization;
        private readonly LocalizationSetting modelLocalization;

        private readonly IBackgroundJobScheduler jobs;

        public CompanyContractController(ApplicationContext dbContext,
                                         ILogger<CompanyContractController> logger,
                                         IOptionsSnapshot<GameSettings> gameOptionsAccessor,
                                         IBackgroundJobScheduler jobScheduler)
        {
            this.ctx = dbContext;
            this.logger = logger;

            this.gameSettings = gameOptionsAccessor.Value;

            this.localization = this.gameSettings.Localization;
            this.modelLocalization = this.localization.Business.Company.CompanyContract;

            this.jobs = jobScheduler;
        }

        ///<inheritdoc/>
        public async Task<GameResult<CompanyContract>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            var model = await ctx.CompanyContracts.FindAsync(id);

            return model;
        }

        public async Task<GameResult<CompanyContract>> Create([FromBody] CompanyContractApiCreationParams pars)
        {
            var item = await ctx.Items.FindAsync(pars.ItemId);
            if (item == null) return Errors.DoesNotExist(pars.ItemId, localization.Crafting.Item);

            var incomeCompany = await ctx.Companies.FindAsync(pars.IncomeCompanyId);
            if (incomeCompany == null) return Errors.DoesNotExist(pars.IncomeCompanyId, localization.Business.Company.Company);

            var outcomeCompany = await ctx.Companies.FindAsync(pars.OutcomeCompanyId);
            if (outcomeCompany == null) return Errors.DoesNotExist(pars.OutcomeCompanyId, localization.Business.Company.Company);

            var creationResult = CompanyContract.Create(new CompanyContractCreationParams
            {
                Name = pars.Name,
                Description = pars.Description,
                Item = item,
                IncomeCompany = incomeCompany,
                OutcomeCompany = outcomeCompany,
                Issuer = pars.Issuer,
                ItemQuantity = pars.ItemQuantity,
                OverallCost = pars.OverallCost,
                TerminationTermInDays = pars.TerminationTermInWeeks
            }, gameSettings);
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ScheduleCompletionCheckerJob(model, jobs);

            ctx.CompanyContracts.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }


        public async Task<GameResult<CompanyContract>> Conclude([FromBody] string json, [FromServices] IBackgroundJobScheduler jobs)
        {
            var pars = new
            {
                contractRequestId = default(long),
                invokerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var contractRequest = await ctx.CompanyContractRequests.FindAsync(d.contractRequestId);
            if (contractRequest == null) return Errors.DoesNotExist(d.contractRequestId, localization.Business.Company.CompanyContractRequest);

            var invoker = await ctx.Players.FindAsync(d.invokerId);

            var creationResult = CompanyContract.Conclude(contractRequest, gameSettings, invoker);
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ScheduleCompletionCheckerJob(model, jobs);

            ctx.CompanyContracts.Add(model);
            ctx.CompanyContractRequests.Remove(contractRequest);
            await ctx.SaveChangesAsync();

            return model;
        }


        public async Task<GameResult<bool>> CheckCompletion([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var companyContract = await ctx.CompanyContracts.FindAsync(d.modelId);
            if (companyContract == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = companyContract.CheckCompletion();

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<int>> AddDeliveredAmount([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                amount = default(int)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var companyContract = await ctx.CompanyContracts.FindAsync(d.modelId);
            if (companyContract == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = companyContract.AddDeliveredAmount(d.amount);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<EmptyGameResult> Complete([FromBody] string json, [FromServices] IBackgroundJobScheduler jobs)
        {
            var pars = new
            {
                modelId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var companyContract = await ctx.CompanyContracts.FindAsync(d.modelId);
            if (companyContract == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = companyContract.Complete();
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            jobs.Remove(companyContract.CompletionCheckerBackgroundJobId);
            companyContract.CompletionCheckerBackgroundJobId = null;

            ctx.CompanyContracts.Remove(companyContract);
            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<EmptyGameResult> Break([FromBody] string json, [FromServices] IBackgroundJobScheduler jobs)
        {
            var pars = new
            {
                modelId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var companyContract = await ctx.CompanyContracts.FindAsync(d.modelId);
            if (companyContract == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            jobs.Remove(companyContract.CompletionCheckerBackgroundJobId);
            companyContract.CompletionCheckerBackgroundJobId = null;

            ctx.Remove(companyContract);
            await ctx.SaveChangesAsync();

            return new EmptyGameResult();
        }

        //public async Task<EmptyGameResult> ScheduleCompletionCheck([FromBody] string json, [FromServices] IBackgroundJobScheduler jobs)
        //{
        //    var pars = new
        //    {
        //        modelId = default(long)
        //    };

        //    var d = JsonConvert.DeserializeAnonymousType(json, pars);

        //    var companyContract = await ctx.CompanyContracts.FindAsync(d.modelId);
        //    if (companyContract == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

        //    ScheduleCompletionCheckerJob(companyContract, jobs);

        //    await ctx.SaveChangesAsync();

        //    return new EmptyGameResult();
        //}



        private void ScheduleCompletionCheckerJob(CompanyContract contract, IBackgroundJobScheduler jobs)
        {
            string jobId = jobs.Schedule<ContractCheckJob, ContractCheckJobParams>(new ContractCheckJobParams
            {
                ContractId = contract.Id
            }, TimeSpan.FromDays(contract.TerminationTermInDays));

            contract.CompletionCheckerBackgroundJobId = jobId;
        }
    }
}
