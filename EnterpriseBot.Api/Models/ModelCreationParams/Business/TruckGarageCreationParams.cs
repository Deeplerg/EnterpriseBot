using EnterpriseBot.Api.Game.Business.Company;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Business
{
    public class TruckGarageCreationParams
    {
        public Company OwningCompany { get; set; }
        public sbyte Capacity { get; set; }
    }
}
