using EnterpriseBot.Api.Game.Donation;
using EnterpriseBot.Api.Game.Essences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnterpriseBot.Api.Models.Common.Enums;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Donation
{
    public class DonationCreationParams
    {
        public Player Player { get; set; }
        public Privilege Privilege { get; set; }
    }
}
