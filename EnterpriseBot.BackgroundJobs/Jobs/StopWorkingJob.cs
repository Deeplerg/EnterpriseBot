using EnterpriseBot.ApiWrapper;
using EnterpriseBot.BackgroundJobs.Abstractions;
using EnterpriseBot.BackgroundJobs.Params;
using System.Threading.Tasks;

namespace EnterpriseBot.BackgroundJobs.Jobs
{
    public class StopWorkingJob : IStopWorkingJob
    {
        private readonly EntbotApi api;

        public StopWorkingJob(EntbotApi api)
        {
            this.api = api;
        }

        public async Task Execute(StopWorkingJobParams pars)
        {
            await api.Business.Job.StopWorking(pars.JobId);
        }
    }
}
