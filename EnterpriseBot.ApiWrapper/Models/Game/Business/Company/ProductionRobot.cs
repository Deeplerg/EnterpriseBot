using EnterpriseBot.ApiWrapper.Models.Game.Storages;
using EnterpriseBot.ApiWrapper.Models.Game.Crafting;

namespace EnterpriseBot.ApiWrapper.Models.Game.Business.Company
{
    public class ProductionRobot
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public Company Company { get; set; }


        public Recipe Recipe { get; set; }
        public CompanyStorage WorkingStorage { get; set;  }
        public bool IsWorkingNow { get; set; }
        public decimal SpeedMultiplier { get; set; }
    }
}
