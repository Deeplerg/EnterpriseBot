using EnterpriseBot.ApiWrapper.Abstractions;
using EnterpriseBot.ApiWrapper.Models.Common.Business;
using EnterpriseBot.ApiWrapper.Models.ModelCreationParams.Business;
using EnterpriseBot.ApiWrapper.Models.Other;
using System;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Categories.Business
{
    public class JobCategory : BusinessCategoryBase<Job>,
                              ICreatableCategory<Job, JobCreationParams>
    {
        private static readonly string categoryName = "job";

        public JobCategory(IApiClient api) : base(api) { }

        /// <inheritdoc/>
        public override async Task<Job> Get(object id)
        {
            var pars = new
            {
                id = id
            };

            var result = await api.Call<Job>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Get).ToLower()
            }, pars);

            return result;
        }

        /// <inheritdoc/>
        public async Task<Job> Create(JobCreationParams pars)
        {
            var result = await api.Call<Job>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Create).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Changes job description
        /// </summary>
        /// <param name="jobId">Job id the description of which to change</param>
        /// <param name="newDesc">New description</param>
        /// <returns>New description</returns>
        public async Task<string> ChangeDescription(long jobId, string newDesc)
        {
            var pars = new
            {
                jobId = jobId,
                newDescription = newDesc
            };

            var result = await api.Call<string>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(ChangeDescription).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Changes job salary
        /// </summary>
        /// <param name="jobId">Job id the salary of which to change</param>
        /// <param name="newSalary">New salary</param>
        /// <returns>New salary</returns>
        public async Task<decimal> ChangeSalary(long jobId, decimal newSalary)
        {
            var pars = new
            {
                jobId = jobId,
                newSalary = newSalary
            };

            var result = await api.Call<decimal>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(ChangeSalary).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Applies for the job
        /// </summary>
        /// <param name="jobId">Job id to apply for</param>
        /// <param name="playerId">Player id - job candidate</param>
        /// <returns><see cref="CandidateForJob"/> instance which represents current candidature</returns>
        public async Task<CandidateForJob> ApplyForJob(long jobId, long playerId)
        {
            var pars = new
            {
                jobId = jobId,
                playerId = playerId
            };

            var result = await api.Call<CandidateForJob>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(ApplyForJob).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Hires a job candidate to the job
        /// </summary>
        /// <param name="companyId">Company id containing</param>
        /// <param name="candidateForJobId"></param>
        /// <returns>The job a candidate was hired to</returns>
        public async Task<Job> Hire(long companyId, long candidateForJobId)
        {
            var pars = new
            {
                companyId = companyId,
                candidateForJobId = candidateForJobId
            };

            var result = await api.Call<Job>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Job).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Fires employee from the job
        /// </summary>
        /// <param name="jobId">Job id to fire from</param>
        /// <returns>Job an employee just got fired from</returns>
        public async Task<Job> Fire(long jobId)
        {
            var pars = new
            {
                jobId = jobId
            };

            var result = await api.Call<Job>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(Fire).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Diminishes speed modifier while also increasing salary
        /// </summary>
        /// <param name="jobId">Job id</param>
        /// <returns>Changed Job</returns>
        public async Task<Job> DiminishSpeedModifier(long jobId)
        {
            var pars = new
            {
                jobId = jobId
            };

            var result = await api.Call<Job>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(DiminishSpeedModifier).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Sets the speed modifier without changing salary
        /// </summary>
        /// <param name="jobId">Job id</param>
        /// <param name="value">New speed modifier</param>
        /// <returns>New speed modifier</returns>
        public async Task<decimal> SetSpeedModifier(long jobId, decimal value)
        {
            var pars = new
            {
                jobId = jobId,
                newValue = value
            };

            var result = await api.Call<decimal>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(SetSpeedModifier).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Pays salary. If the company does not have enough units, remnants of the balance will be paid.
        /// </summary>
        /// <param name="jobId">Job id</param>
        public async Task PaySalary(long jobId)
        {
            var pars = new
            {
                jobId = jobId
            };

            await api.Call(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(PaySalary).ToLower()
            }, pars);
        }

        /// <summary>
        /// Starts producing items process and schedules <see cref="StopWorking(long)"/> with delay of <paramref name="workingTime"/>
        /// </summary>
        /// <param name="jobId">Job id</param>
        /// <param name="workingTime">How long to work (to produce items)</param>
        /// <returns>Job instance after starting working</returns>
        public async Task<Job> StartWorking(long jobId, TimeSpan workingTime)
        {
            var pars = new
            {
                jobId = jobId,
                workingTime = workingTime
            };

            var result = await api.Call<Job>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(StartWorking).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Stops working
        /// </summary>
        /// <param name="jobId">Job id</param>
        /// <returns>Job instance after stopping working</returns>
        public async Task<Job> StopWorking(long jobId)
        {
            var pars = new
            {
                jobId = jobId
            };

            var result = await api.Call<Job>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(StopWorking).ToLower()
            }, pars);

            return result;
        }

        /// <summary>
        /// Directly produces an item and does so continuously until stopped by <see cref="StopWorking"/>.
        /// </summary>
        /// <param name="jobId">Job id</param>
        /// <returns>Job instance after producing the item</returns>
        public async Task<Job> ProduceItemContinuously(long jobId)
        {
            var pars = new
            {
                jobId = jobId
            };

            var result = await api.Call<Job>(new ApiRequestInfo
            {
                CategoryAreaName = categoryAreaName,
                CategoryName = categoryName,
                MethodName = nameof(ProduceItemContinuously).ToLower()
            }, pars);

            return result;
        }
    }
}
