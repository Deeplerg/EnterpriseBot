using EnterpriseBot.ApiWrapper;
using EnterpriseBot.BackgroundJobs.Abstractions;
using EnterpriseBot.BackgroundJobs.Params;
using System.Threading.Tasks;

namespace EnterpriseBot.BackgroundJobs.Jobs
{
    public class PaySalaryJob : IPaySalaryJob
    {
        private readonly EntbotApi api;

        public PaySalaryJob(EntbotApi api)
        {
            this.api = api;
        }

        public async Task Execute(PaySalaryJobParams pars)
        {
            try
            {
#pragma warning disable CS0618 // Type or member is obsolete
                await api.Business.Company.CompanyJob.PaySalary(pars.JobId);
#pragma warning restore CS0618 // Type or member is obsolete
            }
            catch
            {
                await api.Business.Company.CompanyJob.Fire(pars.JobId);
                return;
            }
#pragma warning disable CS0618 // Type or member is obsolete
            await api.Business.Company.CompanyJob.SchedulePaySalary(pars.JobId);
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
