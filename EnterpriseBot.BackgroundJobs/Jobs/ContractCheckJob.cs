using EnterpriseBot.ApiWrapper;
using EnterpriseBot.BackgroundJobs.Abstractions;
using EnterpriseBot.BackgroundJobs.Params;
using System.Threading.Tasks;

namespace EnterpriseBot.BackgroundJobs.Jobs
{
    public class ContractCheckJob : IContractCheckJob
    {
        private readonly EntbotApi api;

        public ContractCheckJob(EntbotApi api)
        {
            this.api = api;
        }

        public async Task Execute(ContractCheckJobParams pars)
        {
            bool completed = await api.Business.Company.CompanyContract.CheckCompletion(pars.ContractId);
            if (completed)
            {
                await api.Business.Company.CompanyContract.Complete(pars.ContractId);
            }
            else
            {
                await api.Business.Company.CompanyContract.Break(pars.ContractId);
            }
        }
    }
}
