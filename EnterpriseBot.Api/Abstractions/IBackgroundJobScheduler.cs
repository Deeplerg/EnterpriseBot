using EnterpriseBot.BackgroundJobs.Abstractions;
using System;

namespace EnterpriseBot.Api.Abstractions
{
    public interface IBackgroundJobScheduler
    {
        string Schedule<TJob, TJobParams>(TJobParams pars, DateTimeOffset enqueueAt) where TJob : IJob<TJobParams>
                                                                                     where TJobParams : class;

        string Schedule<TJob, TJobParams>(TJobParams pars, TimeSpan delay) where TJob : IJob<TJobParams>
                                                                           where TJobParams : class;

        bool Remove(string jobId);
    }
}
