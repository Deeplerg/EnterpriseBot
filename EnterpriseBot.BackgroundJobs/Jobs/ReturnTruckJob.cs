using EnterpriseBot.ApiWrapper;
using EnterpriseBot.BackgroundJobs.Abstractions;
using EnterpriseBot.BackgroundJobs.Params;
using System.Threading.Tasks;

namespace EnterpriseBot.BackgroundJobs.Jobs
{
    public class ReturnTruckJob : IReturnTruckJob
    {
        private readonly EntbotApi api;

        public ReturnTruckJob(EntbotApi api)
        {
            this.api = api;
        }

        public async Task Execute(ReturnTruckJobParams pars)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            await api.Business.Company.Truck.Return(pars.TruckId);
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
