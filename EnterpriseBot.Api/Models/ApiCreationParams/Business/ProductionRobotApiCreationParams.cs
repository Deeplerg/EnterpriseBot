using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.ApiCreationParams.Business
{
    public class ProductionRobotApiCreationParams
    {
        public string Name { get; set; }

        public long CompanyId { get; set; }
        public long RecipeId { get; set; }
        public long CompanyStorageId { get; set; }
    }
}
