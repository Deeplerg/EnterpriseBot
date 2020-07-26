using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnterpriseBot.ApiWrapper.Models.Enums;

namespace EnterpriseBot.ApiWrapper.Models.CreationParams.Donation
{
    public class DonationCreationParams
    {
        public long PlayerId { get; set; }
        public Privilege Privilege { get; set; }
    }
}
