namespace EnterpriseBot.Api.Models.Settings.LocalizationSettings.BusinessLocalization
{
    public class BusinessCompanyLocalizationSettings
    {
        public LocalizationSetting Company { get; set; }
        public LocalizationSetting CompanyContract { get; set; }
        public LocalizationSetting CompanyContractRequest { get; set; }
        public LocalizationSetting CompanyJob { get; set; }
        public LocalizationSetting CompanyJobApplication { get; set; }
        public LocalizationSetting CompanyWorker { get; set; }
        public LocalizationSetting ProductionRobot { get; set; }
        public LocalizationSetting Truck { get; set; }
        public LocalizationSetting TruckGarage { get; set; }
    }
}
