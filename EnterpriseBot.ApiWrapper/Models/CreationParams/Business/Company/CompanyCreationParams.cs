using EnterpriseBot.ApiWrapper.Models.Enums;

namespace EnterpriseBot.ApiWrapper.Models.CreationParams.Business.Company
{
    public class CompanyCreationParams
    {
        public string Name { get; set; }
        public long DescriptionLocalizedStringId { get; set; }
        public long OwnerPlayerId { get; set; }
        public CompanyExtensions Extensions { get; set; }
    }
}
