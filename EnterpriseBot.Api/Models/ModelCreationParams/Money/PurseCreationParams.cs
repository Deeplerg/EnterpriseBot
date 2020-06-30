using EnterpriseBot.Api.Game.Money;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Money
{
    public class PurseCreationParams
    {
        public decimal UnitsAmount { get; set; }
        public decimal BusinessCoinsAmount { get; set; }
    }
}
