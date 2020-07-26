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
    public class CompanyContractRequestController : Controller,
                                                    IGameController<CompanyContractRequest, CompanyContractRequestApiCreationParams>
    {
        private readonly ApplicationContext ctx;
        private readonly ILogger<CompanyContractRequestController> logger;

        private readonly GameSettings gameSettings;

        private readonly LocalizationSettings localization;
        private readonly LocalizationSetting modelLocalization;

        public CompanyContractRequestController(ApplicationContext dbContext,
                                                ILogger<CompanyContractRequestController> logger,
                                                IOptionsSnapshot<GameSettings> gameOptionsAccessor)
        {
            this.ctx = dbContext;
            this.logger = logger;

            this.gameSettings = gameOptionsAccessor.Value;

            this.localization = this.gameSettings.Localization;
            this.modelLocalization = this.localization.Business.Company.CompanyContractRequest;
        }

        ///<inheritdoc/>
        public async Task<GameResult<CompanyContractRequest>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            var model = await ctx.CompanyContractRequests.FindAsync(id);

            return model;
        }

        public async Task<GameResult<CompanyContractRequest>> Create([FromBody] CompanyContractRequestApiCreationParams pars)
        {
            var item = await ctx.Items.FindAsync(pars.ItemId);
            if (item == null) return Errors.DoesNotExist(pars.ItemId, localization.Crafting.Item);

            var requestedCompany = await ctx.Companies.FindAsync(pars.RequestedCompanyId);
            if (requestedCompany == null) return Errors.DoesNotExist(pars.RequestedCompanyId, localization.Business.Company.Company);

            var requestingCompany = await ctx.Companies.FindAsync(pars.RequestingCompanyId);
            if (requestingCompany == null) return Errors.DoesNotExist(pars.RequestingCompanyId, localization.Business.Company.Company);

            var creationResult = CompanyContractRequest.Create(new CompanyContractRequestCreationParams
            {
                Name = pars.Name,
                Description = pars.Description,

                Item = item,
                RequestedCompany = requestedCompany,
                RequestingCompany = requestingCompany,
                RequestingCompanyRelationSide = pars.RequestingCompanyRelationSide,

                ItemQuantity = pars.ItemQuantity,
                OverallCost = pars.OverallCost,
                TerminationTermInDays = pars.TerminationTermInDays
            }, gameSettings);
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.CompanyContractRequests.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }


        public async Task<EmptyGameResult> Decline([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var companyContractRequest = await ctx.CompanyContractRequests.FindAsync(d.modelId);
            if (companyContractRequest == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            ctx.CompanyContractRequests.Remove(companyContractRequest);
            await ctx.SaveChangesAsync();

            return new EmptyGameResult();
        }

        public async Task<GameResult<string>> SetName([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                newName = default(string)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var companyContractRequest = await ctx.CompanyContractRequests.FindAsync(d.modelId);
            if (companyContractRequest == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = companyContractRequest.SetName(d.newName, gameSettings);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<string>> SetDescription([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                newDescription = default(string)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var companyContractRequest = await ctx.CompanyContractRequests.FindAsync(d.modelId);
            if (companyContractRequest == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = companyContractRequest.SetDescription(d.newDescription, gameSettings);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }
    }
}
