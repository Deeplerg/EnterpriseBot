using EnterpriseBot.Api.Game.Business.Company;
using EnterpriseBot.Api.Models.Common.Enums;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Storages
{
    public class CompanyStorageCreationParams
    {
        public decimal Capacity { get; set; }

        public Company OwningCompany { get; set; }

        public CompanyStorageType Type { get; set; }
    }
}
