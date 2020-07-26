using EnterpriseBot.ApiWrapper;
using EnterpriseBot.BackgroundJobs.Abstractions;
using EnterpriseBot.BackgroundJobs.Params;
using System.Threading.Tasks;

namespace EnterpriseBot.BackgroundJobs.Jobs
{
    public class ProduceItemAndStopJob : IProduceItemAndStopJob
    {
        private readonly EntbotApi api;

        public ProduceItemAndStopJob(EntbotApi api)
        {
            this.api = api;
        }

        public async Task Execute(ProduceItemAndStopJobParams pars)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            await api.Business.Company.CompanyWorker.ProduceItem(pars.CompanyWorkerId);
            await api.Business.Company.CompanyWorker.StopWorking(pars.CompanyWorkerId);
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
