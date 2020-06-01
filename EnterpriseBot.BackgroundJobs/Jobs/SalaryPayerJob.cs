using EnterpriseBot.ApiWrapper;
using EnterpriseBot.BackgroundJobs.Abstractions;
using EnterpriseBot.BackgroundJobs.Params;
using System.Threading.Tasks;

namespace EnterpriseBot.BackgroundJobs.Jobs
{
    public class SalaryPayerJob : ISalaryPayerJob
    {
        private readonly EntbotApi api;

        public SalaryPayerJob(EntbotApi api)
        {
            this.api = api;
        }

        public async Task Execute(SalaryPayerJobParams pars)
        {
            await api.Business.Job.PaySalary(pars.JobId);
        }
    }
}
