using EnterpriseBot.BackgroundJobs.Params;

namespace EnterpriseBot.BackgroundJobs.Abstractions
{
    public interface IUnloadTruckJob : IJob<UnloadTruckJobParams> { }
    public interface IReturnTruckJob : IJob<ReturnTruckJobParams> { }
    public interface IContractCheckerJob : IJob<ContractCheckerJobParams> { }
    public interface IContractBreakerJob : IJob<ContractBreakerJobParams> { }
    public interface ISalaryPayerJob : IJob<SalaryPayerJobParams> { }
    public interface IProduceItemJob : IJob<ProduceItemJobParams> { }
    public interface IStopWorkingJob : IJob<StopWorkingJobParams> { }
}
