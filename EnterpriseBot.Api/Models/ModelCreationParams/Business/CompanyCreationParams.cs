namespace EnterpriseBot.Api.Models.ModelCreationParams.Business
{
    public class CompanyCreationParams
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public long GeneralManagerId { get; set; }
        //public decimal CompanyUnits { get; set; } ???

        //public List<long> OutputItemsIds { get; set; }
    }
}
