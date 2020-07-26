using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Attributes;
using EnterpriseBot.Api.Game.Localization;
using EnterpriseBot.Api.Game.Reputation;
using EnterpriseBot.Api.Game.Storages;
using EnterpriseBot.Api.Models.ApiCreationParams.Business;
using EnterpriseBot.Api.Models.Common.Enums;
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
    public class CompanyController : Controller,
                                     IGameController<Game.Business.Company.Company, CompanyApiCreationParams>
    {
        private readonly ApplicationContext ctx;
        private readonly ILogger<CompanyController> logger;

        private readonly GameSettings gameSettings;

        private readonly LocalizationSettings localization;
        private readonly LocalizationSetting modelLocalization;

        public CompanyController(ApplicationContext dbContext,
                                 ILogger<CompanyController> logger,
                                 IOptionsSnapshot<GameSettings> gameOptionsAccessor)
        {
            this.ctx = dbContext;
            this.logger = logger;

            this.gameSettings = gameOptionsAccessor.Value;

            this.localization = this.gameSettings.Localization;
            this.modelLocalization = this.localization.Business.Company.Company;
        }

        ///<inheritdoc/>
        public async Task<GameResult<Game.Business.Company.Company>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            var model = await ctx.Companies.FindAsync(id);

            return model;
        }

        public async Task<GameResult<Game.Business.Company.Company>> Create([FromBody] CompanyApiCreationParams pars)
        {
            var owner = await ctx.Players.FindAsync(pars.OwnerPlayerId);
            if (owner == null) return Errors.DoesNotExist(pars.OwnerPlayerId, localization.Essences.Player);

            var description = await ctx.LocalizedStrings.FindAsync(pars.DescriptionLocalizedStringId);
            if (description == null) return Errors.DoesNotExist(pars.DescriptionLocalizedStringId, localization.Localization.LocalizedString);

            var creationResult = Game.Business.Company.Company.Create(new CompanyCreationParams
            {
                Name = pars.Name,
                Description = description,
                Extensions = pars.Extensions,
                Owner = owner
            }, gameSettings);
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.Companies.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }


        public async Task<GameResult<Game.Business.Company.Company>> BuyAndCreate([FromBody] string json)
        {
            var pars = new
            {
                ownerId = default(long),
                companyCreationParams = default(CompanyApiCreationParams)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var creationPars = d.companyCreationParams;

            var owner = await ctx.Players.FindAsync(d.ownerId);
            if (owner == null) return Errors.DoesNotExist(d.ownerId, localization.Essences.Player);

            var description = await ctx.LocalizedStrings.FindAsync(creationPars.DescriptionLocalizedStringId);
            if (description == null) return Errors.DoesNotExist(creationPars.DescriptionLocalizedStringId, localization.Localization.LocalizedString);

            var buyResult = Game.Business.Company.Company.Buy(creationPars.Extensions, gameSettings, owner);
            if (buyResult.LocalizedError != null) return buyResult.LocalizedError;

            var creationResult = Game.Business.Company.Company.Create(new CompanyCreationParams
            {
                Extensions = creationPars.Extensions,
                Description = description,
                Name = creationPars.Name,
                Owner = owner
            }, gameSettings);
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.Companies.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }

        public async Task<GameResult<decimal>> GetOverallCreationPrice([FromBody] string json)
        {
            var pars = new
            {
                extensions = default(CompanyExtensions),
                ownerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var owner = await ctx.Players.FindAsync(d.ownerId);
            if (owner == null) return Errors.DoesNotExist(d.ownerId, localization.Essences.Player);

            var actionResult = Game.Business.Company.Company.GetOverallCreationPrice(d.extensions, gameSettings, owner);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            return actionResult;
        }

        public async Task<GameResult<CompanyStorage>> GetCompanyStorageWithAvailableSpace([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                space = default(decimal),
                storageType = default(CompanyStorageType)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var company = await ctx.Companies.FindAsync(d.modelId);
            if (company == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = company.GetCompanyStorageWithAvailableSpace(d.space, d.storageType);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            return actionResult;
        }

        public async Task<GameResult<bool>> HasCompanyStorageWithAvailableSpace([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                space = default(decimal),
                storageType = default(CompanyStorageType)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var company = await ctx.Companies.FindAsync(d.modelId);
            if (company == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = company.HasCompanyStorageWithAvailableSpace(d.space, d.storageType);

            return actionResult;
        }

        public async Task<GameResult<decimal>> ReduceBusinessCoins([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                amount = default(decimal),
                invokerId = default(long?)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var company = await ctx.Companies.FindAsync(d.modelId);
            if (company == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var invoker = await ctx.Players.FindAsync(d.invokerId);

            var actionResult = company.ReduceBusinessCoins(d.amount, gameSettings, invoker);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<StringLocalization>> EditDescription([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                newDescription = default(string),
                language = default(LocalizationLanguage)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var company = await ctx.Companies.FindAsync(d.modelId);
            if (company == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = company.EditDescription(d.newDescription, d.language, gameSettings);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<EmptyGameResult> SetOwner([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                newOwnerPlayerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var company = await ctx.Companies.FindAsync(d.modelId);
            if (company == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var newOwner = await ctx.Players.FindAsync(d.newOwnerPlayerId);
            if (newOwner == null) return Errors.DoesNotExist(d.newOwnerPlayerId, localization.Essences.Player);

            var actionResult = company.SetOwner(newOwner);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return new EmptyGameResult();
        }

        //public async Task<EmptyGameResult> CompleteAndRemoveContract([FromBody] string json, [FromServices] IBackgroundJobScheduler jobs)
        //{
        //    var pars = new
        //    {
        //        modelId = default(long),
        //        contractId = default(long)
        //    };

        //    var d = JsonConvert.DeserializeAnonymousType(json, pars);

        //    var company = await ctx.Companies.FindAsync(d.modelId);
        //    if (company == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

        //    var contract = await ctx.CompanyContracts.FindAsync(d.contractId);
        //    if (contract == null) return Errors.DoesNotExist(d.contractId, localization.Business.Company.CompanyContract);

        //    var actionResult = company.CompleteAndRemoveContract(contract);
        //    if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

        //    jobs.Remove(contract.BreakerBackgroundJobId);
        //    jobs.Remove(contract.CompletionCheckerAndDeliveredAmountResetterBackgroundJobId);

        //    await ctx.SaveChangesAsync();

        //    return new EmptyGameResult();
        //}

        public async Task<GameResult<bool>> CanConcludeOneMoreContract([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                invokerId = default(long?)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var company = await ctx.Companies.FindAsync(d.modelId);
            if (company == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var invoker = await ctx.Players.FindAsync(d.invokerId);

            var actionResult = company.CanConcludeOneMoreContract(gameSettings, invoker);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            return actionResult;
        }

        public async Task<GameResult<uint>> GetContractMaxTimeInDays([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                invokerId = default(long?)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var company = await ctx.Companies.FindAsync(d.modelId);
            if (company == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var invoker = await ctx.Players.FindAsync(d.invokerId);

            var actionResult = company.GetContractMaxTimeInDays(gameSettings, invoker);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            return actionResult;
        }

        public async Task<GameResult<Review>> WriteReview([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                text = default(string),
                rating = default(sbyte),
                reputationId = default(long),
                invokerId = default(long?)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var company = await ctx.Companies.FindAsync(d.modelId);
            if (company == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var reputation = await ctx.Reputations.FindAsync(d.reputationId);
            if (reputation == null) return Errors.DoesNotExist(d.reputationId, localization.Reputation.Reputation);

            var invoker = await ctx.Players.FindAsync(d.invokerId);

            var actionResult = company.WriteReview(reputation, d.text, d.rating, gameSettings, invoker);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<Review>> EditReview([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                reviewId = default(long),
                newText = default(string),
                newRating = default(sbyte),
                invokerId = default(long?)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var company = await ctx.Companies.FindAsync(d.modelId);
            if (company == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var review = await ctx.Reviews.FindAsync(d.reviewId);
            if (review == null) return Errors.DoesNotExist(d.reviewId, localization.Reputation.Review);

            var invoker = await ctx.Players.FindAsync(d.invokerId);

            var actionResult = company.EditReview(review, d.newText, d.newRating, gameSettings, invoker);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }
    }
}
