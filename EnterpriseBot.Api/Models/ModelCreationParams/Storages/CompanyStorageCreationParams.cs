using EnterpriseBot.Api.Game.Business.Company;
using EnterpriseBot.Api.Models.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Storages
{
    public class CompanyStorageCreationParams
    {
        public decimal Capacity { get; set; }

        public Company OwningCompany { get; set; }

        public CompanyStorageType Type { get; set; }
    }
}
