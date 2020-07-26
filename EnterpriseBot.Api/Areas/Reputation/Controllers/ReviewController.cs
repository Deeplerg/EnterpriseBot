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
    public class ReviewController : Controller,
                                    IGameController<Review, ReviewApiCreationParams>
    {
        private readonly ApplicationContext ctx;
        private readonly ILogger<ReviewController> logger;

        private readonly GameSettings gameSettings;

        private readonly LocalizationSettings localization;
        private readonly LocalizationSetting modelLocalization;

        public ReviewController(ApplicationContext dbContext,
                                ILogger<ReviewController> logger,
                                IOptionsSnapshot<GameSettings> gameOptionsAccessor)
        {
            this.ctx = dbContext;
            this.logger = logger;

            this.gameSettings = gameOptionsAccessor.Value;

            this.localization = this.gameSettings.Localization;
            this.modelLocalization = this.localization.Reputation.Review;
        }

        public async Task<GameResult<Review>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            var model = await ctx.Reviews.FindAsync(id);

            return model;
        }

        public async Task<GameResult<Review>> Create([FromBody] ReviewApiCreationParams pars)
        {
            var companyReviewer = await ctx.Companies.FindAsync(pars.ReviewerCompanyId);
            var playerReviewer = await ctx.Players.FindAsync(pars.ReviewerPlayerId);

            var creationResult = Review.Create(new ReviewCreationParams
            {
                CompanyReviewer = companyReviewer,
                PlayerReviewer = playerReviewer,
                Reviewer = pars.Reviewer,
                Rating = pars.Rating,
                Text = pars.Text
            }, gameSettings);
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.Reviews.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }


        public async Task<GameResult<Review>> Edit([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                newText = default(string),
                newRating = default(sbyte)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var review = await ctx.Reviews.FindAsync(d.modelId);
            if (review == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = review.Edit(d.newText, d.newRating, gameSettings);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return review;
        }
    }
}
