using EnterpriseBot.ApiWrapper.Models.Common.Business;

namespace EnterpriseBot.ApiWrapper.Models.Common.Essences
{
    public class Bot
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public long JobId { get; set; } //bot here is only able to work and nothing else
        public virtual Job Job { get; set; }
    }
}
