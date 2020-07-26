using EnterpriseBot.Api.Game.Business.Company;
using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Game.Storages;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Business
{
    public class CompanyWorkerCreationParams
    {
        public Company Company { get; set; }
        public Recipe Recipe { get; set; }
        public CompanyStorage CompanyStorage { get; set; }
    }
}
