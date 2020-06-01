using EnterpriseBot.ApiWrapper;
using EnterpriseBot.BackgroundJobs.Abstractions;
using EnterpriseBot.BackgroundJobs.Params;
using System.Threading.Tasks;

namespace EnterpriseBot.BackgroundJobs.Jobs
{
    public class ContractBreakerJob : IContractBreakerJob
    {
        private readonly EntbotApi api;

        public ContractBreakerJob(EntbotApi api)
        {
            this.api = api;
        }

        public async Task Execute(ContractBreakerJobParams pars)
        {
            await api.Business.Contract.Break(pars.ContractId);
        }
    }
}
