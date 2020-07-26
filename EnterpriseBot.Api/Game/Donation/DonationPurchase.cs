using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.ModelCreationParams.Donation;
using EnterpriseBot.Api.Models.Other;
using System;

namespace EnterpriseBot.Api.Game.Donation
{
    public class DonationPurchase
    {
        protected DonationPurchase() { }

        #region model
        public long Id { get; protected set; }

        public DateTime Time { get; protected set; }
        public DonationType Type { get; protected set; }
        public virtual Donation RelatedDonation { get; protected set; }

        public decimal Price { get; protected set; }
        public DonationPriceCurrency Currency { get; protected set; }
        #endregion

        #region actions
        public static GameResult<DonationPurchase> Create(DonationPurchaseCreationParams pars)
        {
            return new DonationPurchase
            {
                Time = pars.Time ?? DateTime.Now,
                Type = pars.Type,
                RelatedDonation = pars.RelatedDonation,

                Price = pars.Price,
                Currency = pars.Currency
            };
        }
        #endregion
    }
}
