namespace EnterpriseBot.Api.Models.Common.Business
{
    public class ContractRequest
    {
        public long Id { get; set; }

        public long ContractInfoId { get; set; }
        public virtual ContractInfo ContractInfo { get; set; }

        public long? RequestedCompanyId { get; set; }
        public virtual Company RequestedCompany { get; set; }

        public long? RequestedShopId { get; set; }
        public virtual Shop RequestedShop { get; set; }
    }
}
