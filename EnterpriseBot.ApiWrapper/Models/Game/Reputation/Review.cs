using EnterpriseBot.ApiWrapper.Models.Game.Business.Company;
using EnterpriseBot.ApiWrapper.Models.Game.Essences;
using EnterpriseBot.ApiWrapper.Models.Enums;

namespace EnterpriseBot.ApiWrapper.Models.Game.Reputation
{
    public class Review
    {
        private Review() { }

        public long Id { get; set; }
        public string Text { get; set; }
        
        public Reviewer Reviewer { get; set; }
        public Company CompanyReviewer { get; set; }
        public Player PlayerReviewer { get; set; }

        public sbyte Rating { get; set; }
    }
}
