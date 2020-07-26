using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnterpriseBot.ApiWrapper.Models.Enums;

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
