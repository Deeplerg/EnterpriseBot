using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Game.Reputation;
using EnterpriseBot.Api.Models.ApiCreationParams.Reputation;
using EnterpriseBot.Api.Models.Contexts;
using EnterpriseBot.Api.Models.ModelCreationParams.Reputation;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Areas.Reputation.Controllers
{
    [Area(nameof(Reputation))]
    public class ReputationController : Controller,
                                        IGameController<Game.Reputation.Reputation, ReputationApiCreationParams>
    {
        private readonly ApplicationContext ctx;
        private readonly ILogger<ReputationController> logger;

        private readonly GameSettings gameSettings;

        private readonly LocalizationSettings localization;
        private readonly LocalizationSetting modelLocalization;

        public ReputationController(ApplicationContext dbContext,
                                    ILogger<ReputationController> logger,
                                    IOptionsSnapshot<GameSettings> gameOptionsAccessor)
        {
            this.ctx = dbContext;
            this.logger = logger;

            this.gameSettings = gameOptionsAccessor.Value;

            this.localization = this.gameSettings.Localization;
            this.modelLocalization = this.localization.Reputation.Reputation;
        }

        public async Task<GameResult<Game.Reputation.Reputation>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            var model = await ctx.Reputations.FindAsync(id);

            return model;
        }

        public async Task<GameResult<Game.Reputation.Reputation>> Create([FromBody] ReputationApiCreationParams pars)
        {
            var creationResult = Game.Reputation.Reputation.Create(new Game.Reputation.ReputationCreationParams
            {
            });
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.Reputations.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }


        public async Task<GameResult<Review>> AddReview([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                reviewCreationParams = default(ReviewApiCreationParams)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var reputation = await ctx.Reputations.FindAsync(d.modelId);
            if (reputation == null) return Errors.DoesNotExist(d.modelId, modelLocalization);


            var reviewPars = d.reviewCreationParams;

            var companyReviewer = await ctx.Companies.FindAsync(reviewPars.ReviewerCompanyId);
            var playerReviewer = await ctx.Players.FindAsync(reviewPars.ReviewerPlayerId);

            var actionResult = reputation.AddReview(new ReviewCreationParams
            {
                CompanyReviewer = companyReviewer,
                PlayerReviewer = playerReviewer,
                Reviewer = reviewPars.Reviewer,
                Rating = reviewPars.Rating,
                Text = reviewPars.Text
            }, gameSettings);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }
    }
}
