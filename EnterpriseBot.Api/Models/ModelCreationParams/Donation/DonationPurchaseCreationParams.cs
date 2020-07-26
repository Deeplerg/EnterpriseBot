using EnterpriseBot.Api.Models.Common.Enums;
using System;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Donation
{
    public class DonationPurchaseCreationParams
    {
        /// <summary>
        /// Optional
        /// </summary>
        public DateTime? Time { get; set; }
        public DonationType Type { get; set; }
        public Game.Donation.Donation RelatedDonation { get; set; }

        public decimal Price { get; set; }
        public DonationPriceCurrency Currency { get; set; }
    }
}
