using EnterpriseBot.ApiWrapper;
using EnterpriseBot.BackgroundJobs.Abstractions;
using EnterpriseBot.BackgroundJobs.Params;
using System.Threading.Tasks;

namespace EnterpriseBot.BackgroundJobs.Jobs
{
    public class UnloadTruckJob : IUnloadTruckJob
    {
        private readonly EntbotApi api;

        public UnloadTruckJob(EntbotApi api)
        {
            this.api = api;
        }

        public async Task Execute(UnloadTruckJobParams pars)
        {
            await api.Storages.IncomeStorage.UnloadTruck(incomeStorageId: pars.IncomeStorageId,
                                                         truckId: pars.TruckId,
                                                         contractId: pars.ContractId);
        }
    }
}
