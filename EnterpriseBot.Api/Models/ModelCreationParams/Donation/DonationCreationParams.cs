using EnterpriseBot.Api.Game.Essences;
using EnterpriseBot.Api.Models.Common.Enums;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Donation
{
    public class DonationCreationParams
    {
        public Player Player { get; set; }
        public Privilege Privilege { get; set; }
    }
}
