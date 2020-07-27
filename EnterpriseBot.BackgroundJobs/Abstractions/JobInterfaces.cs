using EnterpriseBot.BackgroundJobs.Params;

namespace EnterpriseBot.BackgroundJobs.Abstractions
{
    public interface IUnloadTruckJob : IJob<UnloadTruckJobParams> { }
    public interface IReturnTruckJob : IJob<ReturnTruckJobParams> { }
    public interface IContractCheckJob : IJob<ContractCheckJobParams> { }
    public interface IPaySalaryJob : IJob<PaySalaryJobParams> { }
    public interface IProduceItemJob : IJob<ProduceItemJobParams> { }
}
