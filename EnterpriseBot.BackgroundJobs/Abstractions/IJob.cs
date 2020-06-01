using System.Threading.Tasks;

namespace EnterpriseBot.BackgroundJobs.Abstractions
{
    public interface IJob<TParams> where TParams : class
    {
        Task Execute(TParams pars);
    }
}
