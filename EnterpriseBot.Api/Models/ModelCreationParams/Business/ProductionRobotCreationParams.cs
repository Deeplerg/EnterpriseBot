using EnterpriseBot.Api.Game.Business.Company;
using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Game.Storages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Business
{
    public class ProductionRobotCreationParams
    {
        public string Name { get; set; }

        public Company Company { get; set; }
        public Recipe Recipe { get; set; }
        public CompanyStorage Storage { get; set; }
    }
}
