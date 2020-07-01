using EnterpriseBot.Api.Game.Business.Company;
using EnterpriseBot.Api.Game.Essences;
using EnterpriseBot.Api.Models.ModelCreationParams.Reputation;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

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
        #endregion
    }
}
