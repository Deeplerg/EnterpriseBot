using EnterpriseBot.ApiWrapper.Models.Enums;

namespace EnterpriseBot.ApiWrapper.Models.CreationParams.Storages
{
    public class CompanyStorageCreationParams
    {
        public decimal Capacity { get; set; }

        public long OwningCompanyId { get; set; }

        public CompanyStorageType Type { get; set; }
    }
}
