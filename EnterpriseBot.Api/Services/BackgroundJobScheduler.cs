using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.BackgroundJobs.Abstractions;
using Hangfire;
using System;

namespace EnterpriseBot.Api.Services
{
    public class BackgroundJobScheduler : IBackgroundJobScheduler
    {
        private readonly IBackgroundJobClient jobClient;

        public BackgroundJobScheduler(IBackgroundJobClient jobClient)
        {
            this.jobClient = jobClient;
        }

        public string Schedule<TJob, TJobParams>(TJobParams pars, DateTimeOffset enqueueAt) where TJob : IJob<TJobParams>
                                                                                            where TJobParams : class
        {
            return jobClient.Schedule<TJob>(job => job.Execute(pars), enqueueAt);
        }

        public string Schedule<TJob, TJobParams>(TJobParams pars, TimeSpan delay) where TJob : IJob<TJobParams>
                                                                                  where TJobParams : class
        {
            return jobClient.Schedule<TJob>(job => job.Execute(pars), delay);
        }

        public bool Remove(string jobId)
        {
            return jobClient.Delete(jobId);
        }
    }
}
