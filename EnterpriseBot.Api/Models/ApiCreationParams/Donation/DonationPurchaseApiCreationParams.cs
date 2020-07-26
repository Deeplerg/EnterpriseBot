using EnterpriseBot.Api.Models.Common.Enums;
using System;

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
