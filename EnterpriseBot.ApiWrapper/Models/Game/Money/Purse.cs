using System.Collections.Generic;

namespace EnterpriseBot.ApiWrapper.Models.Game.Money
{
    public class Purse
    {
        public long Id { get; set; }
        public List<Money> Money { get; set; } = new List<Money>();
    }
}
