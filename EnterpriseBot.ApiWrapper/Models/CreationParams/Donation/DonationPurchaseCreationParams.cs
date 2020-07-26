using EnterpriseBot.ApiWrapper.Models.Enums;
using System;

namespace EnterpriseBot.ApiWrapper.Models.CreationParams.Donation
{
    public class DonationPurchaseCreationParams
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
