using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Game.Money;
using EnterpriseBot.Api.Models.ApiCreationParams.Money;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.Contexts;
using EnterpriseBot.Api.Models.ModelCreationParams.Money;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Areas.Money.Controllers
{
    [Area(nameof(Money))]
    public class PurseController : Controller,
                                   IGameController<Purse, PurseApiCreationParams>
    {
        private readonly ApplicationContext ctx;
        private readonly ILogger<PurseController> logger;

        private readonly GameSettings gameSettings;

        private readonly LocalizationSettings localization;
        private readonly LocalizationSetting modelLocalization;

        public PurseController(ApplicationContext dbContext,
                               ILogger<PurseController> logger,
                               IOptionsSnapshot<GameSettings> gameOptionsAccessor)
        {
            this.ctx = dbContext;
            this.logger = logger;

            this.gameSettings = gameOptionsAccessor.Value;

            this.localization = this.gameSettings.Localization;
            this.modelLocalization = this.localization.Money.Purse;
        }

        public async Task<GameResult<Purse>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            var model = await ctx.Purses.FindAsync(id);

            return model;
        }

        public async Task<GameResult<Purse>> Create([FromBody] PurseApiCreationParams pars)
        {
            var creationResult = Purse.Create(new PurseCreationParams
            {
                BusinessCoinsAmount = pars.BusinessCoinsAmount,
                UnitsAmount = pars.UnitsAmount
            });
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.Purses.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }

        public async Task<GameResult<decimal>> Add([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                amount = default(decimal),
                currency = default(Currency)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var purse = await ctx.Purses.FindAsync(d.modelId);
            if (purse == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = purse.Add(d.amount, d.currency);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<decimal>> Reduce([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                amount = default(decimal),
                currency = default(Currency)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var purse = await ctx.Purses.FindAsync(d.modelId);
            if (purse == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = purse.Reduce(d.amount, d.currency);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<EmptyGameResult> TransferTo([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                receivingPurseId = default(long),
                amount = default(decimal),
                currency = default(Currency)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var purse = await ctx.Purses.FindAsync(d.modelId);
            if (purse == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var receivingPurse = await ctx.Purses.FindAsync(d.receivingPurseId);
            if (receivingPurse == null) return Errors.DoesNotExist(d.receivingPurseId, modelLocalization);

            var actionResult = purse.TransferTo(receivingPurse, d.amount, d.currency);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<EmptyGameResult> BuyBusinessCoins([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                amount = default(decimal)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var purse = await ctx.Purses.FindAsync(d.modelId);
            if (purse == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = purse.BuyBusinessCoins(d.amount, gameSettings);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }
    }
}
