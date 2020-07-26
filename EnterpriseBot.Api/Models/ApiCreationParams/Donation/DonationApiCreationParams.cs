using EnterpriseBot.Api.Models.Common.Enums;

namespace EnterpriseBot.Api.Models.ApiCreationParams.Donation
{
    public class DonationApiCreationParams
    {
        public long PlayerId { get; set; }
        public Privilege Privilege { get; set; }
    }
}
