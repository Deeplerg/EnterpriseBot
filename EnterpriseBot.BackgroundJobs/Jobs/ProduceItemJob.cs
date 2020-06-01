using EnterpriseBot.ApiWrapper;
using EnterpriseBot.BackgroundJobs.Abstractions;
using EnterpriseBot.BackgroundJobs.Params;
using System.Threading.Tasks;

namespace EnterpriseBot.BackgroundJobs.Jobs
{
    public class ProduceItemJob : IProduceItemJob
    {
        private readonly EntbotApi api;

        public ProduceItemJob(EntbotApi api)
        {
            this.api = api;
        }

        public async Task Execute(ProduceItemJobParams pars)
        {
            await api.Business.Job.ProduceItemContinuously(pars.JobId);
        }
    }
}
