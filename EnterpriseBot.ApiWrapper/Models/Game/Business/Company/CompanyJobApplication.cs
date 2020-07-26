using EnterpriseBot.ApiWrapper.Models.Game.Essences;

namespace EnterpriseBot.ApiWrapper.Models.Game.Business.Company
{
    public class CompanyJobApplication
    {
        public long Id { get; set; }

        public CompanyJob Job { get; set; }
        public Player Applicant { get; set; }

        public string Resume { get; set; }
    }
}
