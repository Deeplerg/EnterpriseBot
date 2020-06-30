using EnterpriseBot.Api.Game.Business.Company;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Storages
{
    public class ShowcaseStorageCreationParams
    {
        public decimal Capacity { get; set; }

        public Company OwningCompany { get; set; }
    }
}
