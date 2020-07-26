namespace EnterpriseBot.ApiWrapper.Models.CreationParams.Business.Company
{
    public class CompanyJobApplicationCreationParams
    {
        public long CompanyJobId { get; set; }
        public long ApplicantPlayerId { get; set; }

        public string Resume { get; set; }
    }
}
