using EnterpriseBot.Api.Game.Business.Company;
using EnterpriseBot.Api.Game.Essences;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Business
{
    public class CompanyJobApplicationCreationParams
    {
        public CompanyJob Job { get; set; }
        public Player Applicant { get; set; }

        public string Resume { get; set; }
    }
}
