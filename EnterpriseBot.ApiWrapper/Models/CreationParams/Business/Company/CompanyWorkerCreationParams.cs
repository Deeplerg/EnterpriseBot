using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Models.CreationParams.Business.Company
{
    public class CompanyWorkerCreationParams
    {
        public long CompanyId { get; set; }
        public long RecipeId { get; set; }
        public long CompanyStorageId { get; set; }
    }
}
