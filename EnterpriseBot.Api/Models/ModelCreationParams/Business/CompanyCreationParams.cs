using EnterpriseBot.Api.Game.Essences;
using EnterpriseBot.Api.Game.Localization;
using EnterpriseBot.Api.Models.Common.Enums;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Business
{
    public class CompanyCreationParams
    {
        public string Name { get; set; }
        public LocalizedString Description { get; set; }

        public Player Owner { get; set; }

        public CompanyExtensions Extensions { get; set; }
    }
}
