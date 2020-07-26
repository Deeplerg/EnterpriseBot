namespace EnterpriseBot.Api.Models.Settings.DonationSettings
{
    public class DonationSettings
    {
        public DonationBusinessPriceMultipliersSetting BusinessPriceMultipliers { get; set; }
        public DonationMaxContractsSetting MaxContracts { get; set; }
        public DonationContractMaxTimeSetting ContractMaxTimeInDays { get; set; }
    }
}
