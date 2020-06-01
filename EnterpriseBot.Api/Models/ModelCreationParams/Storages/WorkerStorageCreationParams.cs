namespace EnterpriseBot.Api.Models.ModelCreationParams.Storages
{
    public class WorkerStorageCreationParams
    {
        //public int CellsAmount { get; set; }
        //public int CellsCapacity { get; set; }
        public int Capacity { get; set; }

        public long OwningCompanyId { get; set; }
    }
}
