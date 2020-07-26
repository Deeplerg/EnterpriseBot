using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Models.CreationParams.Money
{
    public class PurseCreationParams
    {
        public decimal UnitsAmount { get; set; }
        public decimal BusinessCoinsAmount { get; set; }
    }
}
