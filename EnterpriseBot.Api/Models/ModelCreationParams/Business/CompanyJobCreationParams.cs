using EnterpriseBot.Api.Game.Business.Company;
using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Game.Localization;
using EnterpriseBot.Api.Game.Storages;
using EnterpriseBot.Api.Models.Common.Enums;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Business
{
    public class CompanyJobCreationParams
    {
        public LocalizedString Name { get; set; }
        public LocalizedString Description { get; set; }

        public Company Company { get; set; }
        public Recipe Recipe { get; set; }
        public CompanyStorage CompanyStorage { get; set; }

        public CompanyJobPermissions Permissions { get; set; }

        public decimal Salary { get; set; }
    }
}
