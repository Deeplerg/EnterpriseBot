using EnterpriseBot.Api.Game.Donation;
using EnterpriseBot.Api.Game.Essences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Donation
{
    public class DonationCreationParams
    {
        public Privilege Privilege { get; set; }
        public Player Player { get; set; }
    }
}
