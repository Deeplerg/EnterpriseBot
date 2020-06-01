using EnterpriseBot.Api.Models.Common.Essences;

namespace EnterpriseBot.Api.Models.Common.Business
{
    public class CandidateForJob
    {
        public long Id { get; set; }

        public virtual Job Job { get; set; }

        public virtual Player PotentialEmployee { get; set; }

        public long HiringCompanyId { get; set; }
        public virtual Company HiringCompany { get; set; }
    }
}
