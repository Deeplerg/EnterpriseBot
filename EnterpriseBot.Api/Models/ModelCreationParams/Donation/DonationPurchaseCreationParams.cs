using EnterpriseBot.Api.Game.Donation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnterpriseBot.Api.Models.Common.Enums;

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
