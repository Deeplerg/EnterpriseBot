using EnterpriseBot.ApiWrapper.Models.Game.Crafting;
using EnterpriseBot.ApiWrapper.Models.Game.Storages;

namespace EnterpriseBot.ApiWrapper.Models.Game.Business.Company
{
    public class CompanyWorker
    {
        public long Id { get; set; }

        public Company Company { get; set; }

        public Recipe Recipe { get; set; }

        public CompanyStorage WorkingStorage { get; set; }

        public bool IsWorkingNow { get; set; }

        public decimal SpeedMultiplier { get; set; }

        public int LeadTimeInSeconds { get; set; }
    }
}
