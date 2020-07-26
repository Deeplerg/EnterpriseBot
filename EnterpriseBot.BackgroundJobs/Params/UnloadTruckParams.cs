namespace EnterpriseBot.BackgroundJobs.Params
{
    public class UnloadTruckJobParams
    {
        public long TruckId { get; set; }
        public long ContractId { get; set; }
        public long CompanyStorageId { get; set; }
    }
}
