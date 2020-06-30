namespace EnterpriseBot.Api.Models.Settings.BusinessSettings.Company
{
    public class CompanyBusinessSettings
    {
        public decimal DefaultUnits { get; set; }
        public decimal DefaultBusinessCoins { get; set; }

        public CompanyJobSettings Job { get; set; }
        public TruckSettings Truck { get; set; }
        public TruckGarageSettings TruckGarage { get; set; }
        public CompanyWorkerSettings Worker { get; set; }
        public CompanyContractSettings Contract { get; set; }
    }
}
