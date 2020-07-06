using EnterpriseBot.Api.Game.Donation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.ApiCreationParams.Donation
{
    public class DonationPurchaseApiCreationParams
    {
        /// <summary>
        /// Optional
        /// </summary>
        public DateTime? Time { get; set; }
        public DonationType Type { get; set; }
        public long RelatedDonationId { get; set; }

        public decimal Price { get; set; }
        public DonationPriceCurrency Currency { get; set; }
    }
}
