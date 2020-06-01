using EnterpriseBot.Api.Models.Common.Crafting;
using EnterpriseBot.Api.Models.Common.Essences;
using Newtonsoft.Json;

namespace EnterpriseBot.Api.Models.Common.Business
{
    public class Job
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }


        public bool IsOccupied { get; set; }
        public decimal Salary { get; set; }
        //public decimal Prepayment { get; set; } //now the salary will be taken from the Company account


        public long? WorkerId { get; set; } //Job might not be occupied
        public virtual Player Worker { get; set; }

        public long? BotId { get; set; }
        public virtual Bot Bot { get; set; }

        public long CompanyId { get; set; }
        public virtual Company Company { get; set; }

        //public BusinessType BusinessType { get; set; } //no longer need it
        public bool? IsBot { get; set; } //Job might not be occupied

        public long RecipeId { get; set; }
        public virtual Recipe Recipe { get; set; }

        public decimal SpeedModifier { get; set; } //default is 1, decreases with upgrades, increases salary
        public bool? IsWorkingNow { get; set; }

        public int ItemsAmountMadeThisWeek { get; set; }

        [JsonIgnore]
        public string SalaryPayerJobId { get; set; }
        [JsonIgnore]
        public string ProduceItemJobId { get; set; }
        [JsonIgnore]
        public string StopWorkingJobId { get; set; }
    }
}
