using EnterpriseBot.Api.Models.Common.Enums;

namespace EnterpriseBot.Api.Models.ApiCreationParams.Reputation
{
    public class ReviewApiCreationParams
    {
        public string Text { get; set; }

        public Reviewer Reviewer { get; set; }

        public long ReviewerCompanyId { get; set; }
        public long ReviewerPlayerId { get; set; }

        public sbyte Rating { get; set; }
    }
}
