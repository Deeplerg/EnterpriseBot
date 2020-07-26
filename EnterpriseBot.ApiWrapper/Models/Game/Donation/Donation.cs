using EnterpriseBot.ApiWrapper.Models.Enums;
using EnterpriseBot.ApiWrapper.Models.Game.Essences;
using System.Collections.Generic;

namespace EnterpriseBot.ApiWrapper.Models.Game.Donation
{
    public class Donation
    {
        public long Id { get; set; }

        public Privilege Privilege { get; set; }

        public List<DonationPurchase> History { get; set; } = new List<DonationPurchase>();

        public Player Player { get; set; }

        public bool HasDonation { get; set; }
    }
}
