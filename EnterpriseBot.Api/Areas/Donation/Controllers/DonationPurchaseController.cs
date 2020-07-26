using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Models.ApiCreationParams.Donation;
using EnterpriseBot.Api.Models.Contexts;
using EnterpriseBot.Api.Models.ModelCreationParams.Donation;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Areas.DonationPurchase.Controllers
{
    [Area(nameof(Donation))]
    public class DonationPurchaseController : Controller,
                                              IGameController<Game.Donation.DonationPurchase, DonationPurchaseApiCreationParams>
    {
        private readonly ApplicationContext ctx;
        private readonly ILogger<DonationPurchaseController> logger;

        private readonly GameSettings gameSettings;

        private readonly LocalizationSettings localization;
        private readonly LocalizationSetting modelLocalization;

        public DonationPurchaseController(ApplicationContext dbContext,
                                          ILogger<DonationPurchaseController> logger,
                                          IOptionsSnapshot<GameSettings> gameOptionsAccessor)
        {
            this.ctx = dbContext;
            this.logger = logger;

            this.gameSettings = gameOptionsAccessor.Value;

            this.localization = this.gameSettings.Localization;
            this.modelLocalization = this.localization.Donation.DonationPurchase;
        }

        ///<inheritdoc/>
        public async Task<GameResult<Game.Donation.DonationPurchase>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            var model = await ctx.DonationPurchases.FindAsync(id);

            return model;
        }

        public async Task<GameResult<Game.Donation.DonationPurchase>> Create([FromBody] DonationPurchaseApiCreationParams pars)
        {
            var donation = await ctx.Donations.FindAsync(pars.RelatedDonationId);
            if (donation == null) return Errors.DoesNotExist(pars.RelatedDonationId, localization.Donation.DonationPurchase);

            var creationResult = Game.Donation.DonationPurchase.Create(new DonationPurchaseCreationParams
            {
                RelatedDonation = donation,

                Currency = pars.Currency,
                Price = pars.Price,
                Time = pars.Time,
                Type = pars.Type
            });
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.DonationPurchases.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }
    }
}
