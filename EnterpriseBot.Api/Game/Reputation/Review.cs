using EnterpriseBot.Api.Game.Business.Company;
using EnterpriseBot.Api.Game.Essences;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.ModelCreationParams.Reputation;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings;
using EnterpriseBot.Api.Utils;
using static EnterpriseBot.Api.Utils.UserInputUtils;

namespace EnterpriseBot.Api.Game.Reputation
{
    public class Review
    {
        protected Review() { }

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

            switch (pars.Reviewer)
            {
                case Reviewer.Company:
                    if (pars.CompanyReviewer is null)
                        return ReviewerIsNullError(pars.Reviewer);
                    break;

                case Reviewer.Player:
                    if (pars.PlayerReviewer is null)
                        return ReviewerIsNullError(pars.Reviewer);
                    break;
            }

            if (pars.CompanyReviewer != null
             && pars.PlayerReviewer != null)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Critical,
                    EnglishMessage = $"Both CompanyReviewer {pars.CompanyReviewer.Id} and PlayerReviewer {pars.PlayerReviewer.Id} were selected"
                };
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

        public EmptyGameResult Edit(string newText, sbyte newRating, GameSettings gameSettings)
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


        private static LocalizedError ReviewerIsNullError(Reviewer reviewer)
        {
            return new LocalizedError
            {
                ErrorSeverity = ErrorSeverity.Critical,
                EnglishMessage = $"Reviewer {reviewer} is null",
                RussianMessage = $"Рецензент {reviewer} является null"
            };
        }
        #endregion
    }
}
