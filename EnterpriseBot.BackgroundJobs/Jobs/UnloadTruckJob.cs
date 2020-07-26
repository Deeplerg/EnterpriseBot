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
#pragma warning disable CS0618 // Type or member is obsolete
            await api.Business.Company.Truck.Unload(pars.TruckId, pars.CompanyStorageId, pars.ContractId);
            await api.Business.Company.Truck.ScheduleReturnTruck(pars.TruckId);
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
