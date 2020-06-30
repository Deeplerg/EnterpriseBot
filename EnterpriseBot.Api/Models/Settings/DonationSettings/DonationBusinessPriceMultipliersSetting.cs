using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.Settings.DonationSettings
{
    public class DonationBusinessPriceMultipliersSetting
    {
        public decimal NoDonation { get; set; }
        public decimal Pro { get; set; }
        public decimal VIP { get; set; }
        public decimal Premium { get; set; }
        public decimal Mega { get; set; }
        public decimal Gold { get; set; }
    }
}
