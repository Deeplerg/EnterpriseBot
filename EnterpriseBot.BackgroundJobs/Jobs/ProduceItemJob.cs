using EnterpriseBot.ApiWrapper;
using EnterpriseBot.BackgroundJobs.Abstractions;
using EnterpriseBot.BackgroundJobs.Params;
using System.Threading;
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
#pragma warning disable CS0618 // Type or member is obsolete
            await api.Business.Company.CompanyWorker.ProduceItem(pars.CompanyWorkerId);
#pragma warning restore CS0618 // Type or member is obsolete

            if(pars.Repeatedly)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                await api.Business.Company.CompanyWorker.ScheduleProduceItem(pars.CompanyWorkerId, pars.Repeatedly);
#pragma warning restore CS0618 // Type or member is obsolete
            }
            else
            {
#pragma warning disable CS0618 // Type or member is obsolete
                await api.Business.Company.CompanyWorker.StopWorking(pars.CompanyWorkerId);
#pragma warning restore CS0618 // Type or member is obsolete
            }
        }
    }
}
