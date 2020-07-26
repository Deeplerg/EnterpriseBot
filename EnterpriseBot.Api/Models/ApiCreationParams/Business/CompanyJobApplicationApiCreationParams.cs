namespace EnterpriseBot.Api.Models.ApiCreationParams.Business
{
    public class CompanyJobApplicationApiCreationParams
    {
        public long CompanyJobId { get; set; }
        public long ApplicantPlayerId { get; set; }

        public string Resume { get; set; }
    }
}
