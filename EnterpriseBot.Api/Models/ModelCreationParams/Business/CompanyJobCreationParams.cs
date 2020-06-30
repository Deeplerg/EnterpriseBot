using EnterpriseBot.Api.Game.Business.Company;
using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Game.Storages;
using EnterpriseBot.Api.Models.Common.Enums;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Business
{
    public class CompanyJobCreationParams
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Company Company { get; set; }
        public Recipe Recipe { get; set; }
        public CompanyStorage Storage { get; set; }

        public bool HasRecipe { get; set; }

        public CompanyJobPermissions Permissions { get; set; }
    }
}
