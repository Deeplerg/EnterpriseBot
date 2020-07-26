using EnterpriseBot.ApiWrapper.Models.Enums;
using System;

namespace EnterpriseBot.ApiWrapper.Models.Game.Donation
{
    public class DonationPurchase
    {
        public long Id { get; set; }

        public DateTime Time { get; set; }
        public DonationType Type { get; set; }
        public Donation RelatedDonation { get; set; }

        public decimal Price { get; set; }
        public DonationPriceCurrency Currency { get; set; }
    }
}
