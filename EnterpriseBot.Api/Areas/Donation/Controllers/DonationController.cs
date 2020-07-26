using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Game.Donation;
using EnterpriseBot.Api.Game.Storages;
using EnterpriseBot.Api.Models.ApiCreationParams.Donation;
using EnterpriseBot.Api.Models.ApiCreationParams.Storages;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.Contexts;
using EnterpriseBot.Api.Models.ModelCreationParams.Donation;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Areas.Donation.Controllers
{
    [Area(nameof(Donation))]
    public class DonationController : Controller,
                                      IGameController<Game.Donation.Donation, DonationApiCreationParams>
    {
        private readonly ApplicationContext ctx;
        private readonly ILogger<DonationController> logger;

        private readonly GameSettings gameSettings;

        private readonly LocalizationSettings localization;
        private readonly LocalizationSetting modelLocalization;

        public DonationController(ApplicationContext dbContext,
                                  ILogger<DonationController> logger,
                                  IOptionsSnapshot<GameSettings> gameOptionsAccessor)
        {
            this.ctx = dbContext;
            this.logger = logger;

            this.gameSettings = gameOptionsAccessor.Value;

            this.localization = this.gameSettings.Localization;
            this.modelLocalization = this.localization.Donation.Donation;
        }

        ///<inheritdoc/>
        public async Task<GameResult<Game.Donation.Donation>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            var model = await ctx.Donations.FindAsync(id);

            return model;
        }

        public async Task<GameResult<Game.Donation.Donation>> Create([FromBody] DonationApiCreationParams pars)
        {
            var player = await ctx.Players.FindAsync(pars.PlayerId);
            if (player == null) return Errors.DoesNotExist(pars.PlayerId, localization.Essences.Player);

            var creationResult = Game.Donation.Donation.Create(new DonationCreationParams
            {
                Player = player,
                Privilege = pars.Privilege
            });
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.Donations.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }

        public Task<GameResult<decimal>> GetBusinessPriceMultiplierForPrivilege([FromBody] string json)
        {
            var pars = new
            {
                privilege = default(Privilege)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            return Task.FromResult(Game.Donation.Donation.GetBusinessPriceMultiplierForPrivelege(d.privilege, gameSettings));
        }

        public Task<GameResult<uint>> GetMaxContractsForPrivilege([FromBody] string json)
        {
            var pars = new
            {
                privilege = default(Privilege)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            return Task.FromResult(Game.Donation.Donation.GetMaximumContractsForPrivelege(d.privilege, gameSettings));
        }

        public Task<GameResult<uint>> GetContractMaxTimeInDaysForPrivilege([FromBody] string json)
        {
            var pars = new
            {
                privilege = default(Privilege)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            return Task.FromResult(Game.Donation.Donation.GetContractMaxTimeInDaysForPrivilege(d.privilege, gameSettings));
        }


        public async Task<GameResult<decimal>> GetBusinessPriceMultiplier([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var donation = await ctx.Donations.FindAsync(d.modelId);
            if (donation == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            return donation.GetBusinessPriceMultiplier(gameSettings);
        }

        public async Task<GameResult<uint>> GetMaxContracts([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var donation = await ctx.Donations.FindAsync(d.modelId);
            if (donation == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            return donation.GetMaximumContracts(gameSettings);
        }

        public async Task<GameResult<uint>> GetContractMaxTimeInDays([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var donation = await ctx.Donations.FindAsync(d.modelId);
            if (donation == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            return donation.GetContractMaxTimeInDays(gameSettings);
        }

        public async Task<GameResult<Privilege>> UpgradePrivilege([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                privilege = default(Privilege)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var donation = await ctx.Donations.FindAsync(d.modelId);
            if (donation == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = donation.UpgradePrivilege(d.privilege);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<Game.Donation.DonationPurchase>> AddPurchase([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                donationPurchaseParams = default(DonationPurchaseApiCreationParams)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var creationPars = d.donationPurchaseParams;

            var donation = await ctx.Donations.FindAsync(d.modelId);
            if (donation == null) return Errors.DoesNotExist(d.modelId, modelLocalization);


            var actionResult = donation.AddPurchase(new DonationPurchaseCreationParams
            {
                RelatedDonation = donation,

                Currency = creationPars.Currency,
                Price = creationPars.Price,
                Time = creationPars.Time,
                Type = creationPars.Type
            });
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<bool>> CanUpgradeToPrivilege([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                privilege = default(Privilege)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var donation = await ctx.Donations.FindAsync(d.modelId);
            if (donation == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            return donation.CanUpgradeToPrivilege(d.privilege);
        }
    }
}
