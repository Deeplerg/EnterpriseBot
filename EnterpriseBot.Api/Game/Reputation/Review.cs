using EnterpriseBot.Api.Game.Business.Company;
using EnterpriseBot.Api.Game.Essences;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.ModelCreationParams.Reputation;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using static EnterpriseBot.Api.Utils.UserInputUtils;

namespace EnterpriseBot.Api.Game.Reputation
{
    public class Review
    {
        private Review() { }

        #region model
        public long Id { get; protected set; }
        public string Text { get; protected set; }
        
        public Reviewer Reviewer { get; protected set; }
        public virtual Company CompanyReviewer { get; protected set; }
        public virtual Player PlayerReviewer { get; protected set; }

        public sbyte Rating { get; protected set; }

        #region errors
        private static readonly LocalizedError ratingOutOfRangeError = new LocalizedError
        {
            ErrorSeverity = ErrorSeverity.Normal,
            EnglishMessage = "Rating can't be lower than 0, equal to 0 or larger than 5",
            RussianMessage = "Оценка не может быть ниже 0, равна 0 или больше 5"
        };
        #endregion
        #endregion

        #region actions
        public static GameResult<Review> Create(ReviewCreationParams pars, GameSettings gameSettings)
        {
            var req = gameSettings.Localization.UserInputRequirements;

            if (!CheckReviewText(pars.Text))
            {
                return Errors.IncorrectReviewInput(req);
            }

            if (pars.Rating <= 0 || pars.Rating > 5)
            {
                return ratingOutOfRangeError;
            }

            return new Review
            {
                Reviewer = pars.Reviewer,
                CompanyReviewer = pars.CompanyReviewer,
                PlayerReviewer = pars.PlayerReviewer,

                Text = pars.Text,
                Rating = pars.Rating
            };
        }

        public EmptyGameResult Change(string newText, sbyte newRating, GameSettings gameSettings)
        {
            var req = gameSettings.Localization.UserInputRequirements;

            if (!CheckReviewText(newText))
            {
                return Errors.IncorrectReviewInput(req);
            }

            if (newRating <= 0 || newRating > 5)
            {
                return ratingOutOfRangeError;
            }

            Text = newText;
            return new EmptyGameResult();
        }
        #endregion
    }
}
