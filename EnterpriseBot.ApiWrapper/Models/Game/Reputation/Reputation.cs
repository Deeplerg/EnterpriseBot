using System.Collections.Generic;

namespace EnterpriseBot.ApiWrapper.Models.Game.Reputation
{
    public class Reputation
    {
        protected Reputation() { }

        public long Id { get; set; }

        public List<Review> Reviews { get; set; } = new List<Review>();

        public int Rating { get; set; }
    }
}
