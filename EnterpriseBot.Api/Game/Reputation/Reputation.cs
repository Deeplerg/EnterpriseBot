using EnterpriseBot.Api.Models.ModelCreationParams.Reputation;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EnterpriseBot.Api.Game.Reputation
{
    public class Reputation
    {
        protected Reputation() { }

        #region model
        public long Id { get; protected set; }

        public virtual IReadOnlyCollection<Review> Reviews
        {
            get => new ReadOnlyCollection<Review>(reviews);
            protected set => reviews = value.ToList();
        }
        private List<Review> reviews = new List<Review>();

        public int Rating
        {
            get => (int)Reviews.Average(r => r.Rating);
        }
        #endregion

        #region actions
        public static GameResult<Reputation> Create(ReputationCreationParams pars)
        {
            return new Reputation
            {
            };
        }

        public GameResult<Review> AddReview(ReviewCreationParams pars, GameSettings gameSettings)
        {
            var creationResult = Review.Create(pars, gameSettings);
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            Review review = creationResult;

            reviews.Add(review);

            return review;
        }
        #endregion
    }
}
