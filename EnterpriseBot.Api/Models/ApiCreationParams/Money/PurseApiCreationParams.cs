using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.ApiCreationParams.Money
{
    public class PurseApiCreationParams
    {
        public decimal UnitsAmount { get; set; }
        public decimal BusinessCoinsAmount { get; set; }
    }
}
