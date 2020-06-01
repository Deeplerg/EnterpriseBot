using EnterpriseBot.ApiWrapper;
using EnterpriseBot.BackgroundJobs.Abstractions;
using EnterpriseBot.BackgroundJobs.Params;
using System.Threading.Tasks;

namespace EnterpriseBot.BackgroundJobs.Jobs
{
    public class ContractCheckerJob : IContractCheckerJob
    {
        private readonly EntbotApi api;

        public ContractCheckerJob(EntbotApi api)
        {
            this.api = api;
        }

        public async Task Execute(ContractCheckerJobParams pars)
        {
            await api.Business.Contract.CheckContractCompletionAndBreak(contractId: pars.ContractId);
        }
    }
}
