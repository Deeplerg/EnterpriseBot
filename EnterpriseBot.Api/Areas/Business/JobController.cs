using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Models.Common.Business;
using EnterpriseBot.Api.Models.Common.Crafting;
using EnterpriseBot.Api.Models.Common.Essences;
using EnterpriseBot.Api.Models.Common.Storages;
using EnterpriseBot.Api.Models.Contexts;
using EnterpriseBot.Api.Models.ModelCreationParams.Business;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings.GameplaySettings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using EnterpriseBot.BackgroundJobs.Abstractions;
using EnterpriseBot.BackgroundJobs.Jobs;
using EnterpriseBot.BackgroundJobs.Params;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EnterpriseBot.Api.Utils.Constants;
using static EnterpriseBot.Api.Utils.Miscellaneous;

namespace EnterpriseBot.Api.Areas.Business
{
    [Area(nameof(Business))]
    public class JobController : Controller, IGameController<Job>
    {
        private readonly ApplicationContext ctx;
        private readonly GameplaySettings gameplaySettings;
        private readonly LocalizationSettings localizationSettings;

        private readonly LocalizationSetting modelLocalization;
        private readonly string englishModelName;
        private readonly string russianModelName;

        public LocalizationSettings LocalizationSettings => localizationSettings;

        public JobController(ApplicationContext context,
                             IOptions<GameplaySettings> gameplayOptions,
                             IOptions<LocalizationSettings> localizationOptions)
        {
            this.ctx = context;
            this.gameplaySettings = gameplayOptions.Value;
            this.localizationSettings = localizationOptions.Value;

            modelLocalization = localizationSettings.Business.Job;
            englishModelName = modelLocalization.English;
            russianModelName = modelLocalization.Russian;
        }

        /// <inheritdoc/>
        [HttpPost]
        public async Task<GameResult<Job>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            Job job = await ctx.Jobs.FindAsync(id);
            //if (job == null) return JobDoesNotExist(id);

            return job;
        }

        /////<inheritdoc/>
        [HttpPost]
        public async Task<GameResult<Job>> Create([FromBody] JobCreationParams cp)
        {
            UserInputRequirements req = localizationSettings.UserInputRequirements;

            if (!CheckName(cp.Name))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The name has not passed verification. " + string.Format(req.Name.English, NameMaxLength),
                    RussianMessage = "Название не прошло проверку. " + string.Format(req.Name.Russian, NameMaxLength)
                };
            }
            if (!CheckDescription(cp.Description))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The description has not passed verification. " + string.Format(req.Name.English, NameMaxLength),
                    RussianMessage = "Описание не прошло проверку. " + string.Format(req.Name.Russian, NameMaxLength)
                };
            }

            if (cp.Salary < gameplaySettings.Job.MinSalary)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = $"Salary can't be lower than {gameplaySettings.Job.MinSalary}",
                    RussianMessage = $"Зарплата не может быть ниже {gameplaySettings.Job.MinSalary}"
                };
            }

            Company company = await ctx.Companies.FindAsync(cp.CompanyId);
            if (company == null) return Errors.DoesNotExist(cp.CompanyId, modelLocalization);

            Recipe recipe = await ctx.Recipes.FindAsync(cp.RecipeId);
            if (recipe == null) return Errors.DoesNotExist(cp.RecipeId, localizationSettings.Crafting.Recipe);

            if (!company.OutputItems.Contains(recipe.ResultItem))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The company can't produce this item",
                    RussianMessage = "Компания не может произвести этот предмет"
                };
            }

            Job createdJob = (await ctx.Jobs.AddAsync(new Job
            {
                Name = cp.Name,
                Description = cp.Description,
                Company = company,
                Recipe = recipe,
                IsOccupied = false,
                Salary = cp.Salary,
                SpeedModifier = gameplaySettings.Job.DefaultSpeedModifier
            })).Entity;

            await ctx.SaveChangesAsync();

            return await ctx.Jobs.FindAsync(createdJob.Id);
        }

        /// <summary>
        /// Changes job description
        /// </summary>
        /// <returns>New description</returns>
        [HttpPost]
        public async Task<GameResult<string>> ChangeDescription([FromBody] string json)
        {
            var pars = new
            {
                jobId = default(long),
                newDescription = default(string)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            UserInputRequirements req = localizationSettings.UserInputRequirements;

            if (!CheckDescription(d.newDescription))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The description has not passed verification. " + string.Format(req.Name.English, NameMaxLength),
                    RussianMessage = "Описание не прошло проверку. " + string.Format(req.Name.Russian, NameMaxLength)
                };
            }

            Job job = await ctx.Jobs.FindAsync(d.jobId);
            if (job == null) return JobDoesNotExist(d.jobId);

            job.Description = d.newDescription;

            await ctx.SaveChangesAsync();

            return job.Description;
        }

        /// <summary>
        /// Changes job salary
        /// </summary>
        /// <returns>New salary</returns>
        [HttpPost]
        public async Task<GameResult<decimal>> ChangeSalary([FromBody] string json)
        {
            var pars = new
            {
                jobId = default(long),
                newSalary = default(decimal)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            if (d.newSalary < gameplaySettings.Job.MinSalary)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = $"Salary can't be lower than {gameplaySettings.Job.MinSalary}",
                    RussianMessage = $"Зарплата не может быть ниже, чем {gameplaySettings.Job.MinSalary}"
                };
            }

            Job job = await ctx.Jobs.FindAsync(d.jobId);
            if (job == null) JobDoesNotExist(d.jobId);

            job.Salary = d.newSalary;

            await ctx.SaveChangesAsync();

            return job.Salary;
        }

        /// <summary>
        /// Applies for the job
        /// </summary>
        /// <returns><see cref="CandidateForJob"/> instance which represents current candidature</returns>
        [HttpPost]
        public async Task<GameResult<CandidateForJob>> ApplyForJob([FromBody] string json)
        {
            var pars = new
            {
                jobId = default(long),
                playerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Job job = await ctx.Jobs.FindAsync(d.jobId);
            if (job == null) return JobDoesNotExist(d.jobId);

            if (job.IsOccupied)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The job is already occupied",
                    RussianMessage = "Работа уже занята"
                };
            }

            Player player = await ctx.Players.FindAsync(d.playerId);
            if (player == null) return Errors.DoesNotExist(d.playerId, localizationSettings.Essences.Player);

            Company company = job.Company;
            if (company == null) return Errors.DoesNotExist(job.CompanyId, localizationSettings.Business.Company);

            CandidateForJob candidate = (await ctx.CandidatesForJob.AddAsync(new CandidateForJob
            {
                Job = job,
                HiringCompany = company,
                PotentialEmployee = player
            })).Entity;

            await ctx.SaveChangesAsync();

            return await ctx.CandidatesForJob.FindAsync(candidate.Id);
        }

        /// <summary>
        /// Hires a job candidate to the job
        /// </summary>
        /// <returns>The job a candidate was hired to</returns>
        [HttpPost]
        public async Task<GameResult<Job>> Hire([FromBody] string json, [FromServices] BackgroundJobClient backgroundJobClient)
        {
            var pars = new
            {
                companyId = default(long),
                candidateForJobId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Company company = await ctx.Companies.FindAsync(d.companyId);
            if (company == null) return Errors.DoesNotExist(d.companyId, localizationSettings.Business.Company);

            CandidateForJob candidate = await ctx.CandidatesForJob.FindAsync(d.candidateForJobId);
            if (candidate == null) return Errors.DoesNotExist(d.candidateForJobId, localizationSettings.Business.CandidateForJob);

            Job job = candidate.Job;

            if (job.IsOccupied)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The job is already occupied",
                    RussianMessage = "Работа уже занята"
                };
            }

            job.Worker = candidate.PotentialEmployee;
            job.IsOccupied = true;
            job.IsBot = false;
            job.IsWorkingNow = false;

            var salaryPayerJobParams = new SalaryPayerJobParams
            {
                JobId = job.Id
            };
            string salaryPayerJobId = backgroundJobClient.Schedule<SalaryPayerJob>(job => job.Execute(salaryPayerJobParams), TimeSpan.FromDays(7));

            job.SalaryPayerJobId = salaryPayerJobId;

            var candidates = company.Candidates.Where(c => c.Job.Id == job.Id);

            ctx.CandidatesForJob.RemoveRange(candidates);

            await ctx.SaveChangesAsync();

            return await ctx.Jobs.FindAsync(job.Id);
        }

        /// <summary>
        /// Fires employee from the job
        /// </summary>
        /// <returns>Job an employee just got fired from</returns>
        [HttpPost]
        public async Task<GameResult<Job>> Fire([FromBody] string json, [FromServices] BackgroundJobClient backgroundJobClient)
        {
            var pars = new
            {
                jobId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Job job = await ctx.Jobs.FindAsync(d.jobId);
            if (job == null) return JobDoesNotExist(d.jobId);

            job.Worker = null;
            job.Bot = null;
            job.IsBot = null;
            job.IsOccupied = false;
            job.ItemsAmountMadeThisWeek = 0;

            if (job.ProduceItemJobId != null)
                backgroundJobClient.Delete(job.ProduceItemJobId);

            backgroundJobClient.Delete(job.SalaryPayerJobId);

            if (job.StopWorkingJobId != null)
                backgroundJobClient.Delete(job.StopWorkingJobId);

            job.IsWorkingNow = false;

            job.ProduceItemJobId = null;
            job.SalaryPayerJobId = null;
            job.StopWorkingJobId = null;

            await ctx.SaveChangesAsync();
            return await ctx.Jobs.FindAsync(job.Id);
        }

        /// <summary>
        /// Diminishes speed modifier while also increasing salary
        /// </summary>
        /// <returns>Changed Job</returns>
        [HttpPost]
        public async Task<GameResult<Job>> DiminishSpeedModifier([FromBody] string json)
        {
            var pars = new
            {
                jobId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Job job = await ctx.Jobs.FindAsync(d.jobId);
            if (job == null) return JobDoesNotExist(d.jobId);

            if (job.SpeedModifier - gameplaySettings.Job.DiminishModifierStep < gameplaySettings.Job.MinSpeedModifier)
            {
                return new GameResult<Job>
                {
                    LocalizedError = new LocalizedError
                    {
                        ErrorSeverity = ErrorSeverity.Normal,
                        EnglishMessage = "Can't diminsh speed modifier, it is already at the minimum",
                        RussianMessage = "Модификатор скорости не может быть уменьшен, он уже на минимуме"
                    }
                };
            }

            job.SpeedModifier -= gameplaySettings.Job.DiminishModifierStep;
            job.Salary += gameplaySettings.Job.DiminishModifierSalaryIncrease;

            await ctx.SaveChangesAsync();

            return await ctx.Jobs.FindAsync(job.Id);
        }

        /// <summary>
        /// Sets the speed modifier without changing salary
        /// </summary>
        /// <returns>New speed modifier</returns>
        [HttpPost]
        public async Task<GameResult<decimal>> SetSpeedModifier([FromBody] string json)
        {
            var pars = new
            {
                jobId = default(long),
                newValue = default(decimal)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Job job = await ctx.Jobs.FindAsync(d.jobId);
            if (job == null) return JobDoesNotExist(d.jobId);

            if (d.newValue <= 0)
            {
                return new GameResult<decimal>
                {
                    LocalizedError = new LocalizedError
                    {
                        ErrorSeverity = ErrorSeverity.Normal, //Critical???
                        EnglishMessage = "New speed modifier can't be below or equal 0",
                        RussianMessage = "Новый модификатор скорости не может быть ниже или равен 0"
                    }
                };
            }

            job.SpeedModifier = d.newValue;

            await ctx.SaveChangesAsync();

            return (await ctx.Jobs.FindAsync(job.Id)).SpeedModifier;
        }

        /// <summary>
        /// Pays salary. If the company does not have enough units, remnants of the balance will be paid.
        /// </summary>
        [HttpPost]
        public async Task<EmptyGameResult> PaySalary([FromBody] string json, [FromServices] BackgroundJobClient backgroundJobClient)
        {
            var pars = new
            {
                jobId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Job job = await ctx.Jobs.FindAsync(d.jobId);
            if (job == null) return JobDoesNotExist(d.jobId);

            Company company = job.Company;
            if (company == null) return Errors.DoesNotExist(d.jobId, localizationSettings.Business.Company);

            decimal toPay;
            if (company.CompanyUnits < job.Salary)
                toPay = company.CompanyUnits;
            else
                toPay = job.Salary;

            company.CompanyUnits -= toPay;
            if (job.IsBot.HasValue && job.IsBot.Value == false)
            {
                Player player = job.Worker;
                if (player == null) return Errors.DoesNotExist(job.WorkerId, localizationSettings.Essences.Player);

                player.Units += toPay;
            }

            var salaryPayerJobParams = new SalaryPayerJobParams
            {
                JobId = job.Id
            };
            string salaryPayerJobId = backgroundJobClient.Schedule<ISalaryPayerJob>(job => job.Execute(salaryPayerJobParams), TimeSpan.FromDays(7));

            job.SalaryPayerJobId = salaryPayerJobId;
            job.ItemsAmountMadeThisWeek = 0; //it isn't necessary to add new job for this

            await ctx.SaveChangesAsync();

            return new EmptyGameResult();
        }

        /// <summary>
        /// Starts producing items process
        /// </summary>
        /// <returns>Job instance after starting working</returns>
        [HttpPost]
        public async Task<GameResult<Job>> StartWorking([FromBody] string json, [FromServices] BackgroundJobClient backgroundJobClient)
        {
            var pars = new
            {
                jobId = default(long),
                workingTime = default(TimeSpan)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Job job = await ctx.Jobs.FindAsync(d.jobId);
            if (job == null) return JobDoesNotExist(d.jobId);

            if (job.IsWorkingNow.Value == true)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Work is already underway",
                    RussianMessage = "Работа уже идет"
                };
            }

            if (job.IsBot.HasValue && job.IsBot.Value == false && d.workingTime > TimeSpan.FromHours(gameplaySettings.Job.MaxWorkingHoursPlayer))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = $"Working time is above the maximum! Max hours for a player: {gameplaySettings.Job.MaxWorkingHoursPlayer}",
                    RussianMessage = $"Время работы выше максимума! Максимум часов для игрока: {gameplaySettings.Job.MaxWorkingHoursPlayer}"
                };
            }
            else if (job.IsBot.HasValue && job.IsBot.Value == true && d.workingTime > TimeSpan.FromHours(gameplaySettings.Job.MaxWorkingHoursBot))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = $"Working time is above the maximum! Max hours for a bot: {gameplaySettings.Job.MaxWorkingHoursBot}",
                    RussianMessage = $"Время работы выше максимума! Максимум часов для бота: {gameplaySettings.Job.MaxWorkingHoursBot}"
                };
            }

            var incomeStorage = job.Company.IncomeStorage;
            var currentSpaceOccupied = incomeStorage.Items.Sum(storageItem => storageItem.Quantity * storageItem.Item.Space);
            if (incomeStorage.Capacity < currentSpaceOccupied + (job.Recipe.ResultItemQuantity * job.Recipe.ResultItem.Space))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Worker storage does not have enough space to start working",
                    RussianMessage = "На рабочем складе недостаточно места, чтобы начать работать"
                };
            }

            var time = TimeSpan.FromSeconds((double)(job.Recipe.LeadTimeInSeconds * job.SpeedModifier));
            var produceItemJobParams = new ProduceItemJobParams
            {
                JobId = job.Id
            };
            string produceItemJobId = backgroundJobClient.Schedule<ProduceItemJob>(j => j.Execute(produceItemJobParams), delay: time);
            job.ProduceItemJobId = produceItemJobId;

            var stopWorkingJobParams = new StopWorkingJobParams
            {
                JobId = job.Id
            };
            string stopWorkingJobId = backgroundJobClient.Schedule<StopWorkingJob>(j => j.Execute(stopWorkingJobParams), delay: d.workingTime);
            job.StopWorkingJobId = stopWorkingJobId;

            job.IsWorkingNow = true;

            await ctx.SaveChangesAsync();

            return await ctx.Jobs.FindAsync(d.jobId);
        }

        /// <summary>
        /// Stops working
        /// </summary>
        /// <returns>Job instance after stopping working</returns>
        [HttpPost]
        public async Task<GameResult<Job>> StopWorking([FromBody] string json, [FromServices] BackgroundJobClient backgroundJobClient)
        {
            var pars = new
            {
                jobId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Job job = await ctx.Jobs.FindAsync(d.jobId);
            if (job == null) return JobDoesNotExist(d.jobId);

            if (job.IsWorkingNow.HasValue && job.IsWorkingNow.Value == false)
            {
                return new GameResult<Job>
                {
                    LocalizedError = new LocalizedError
                    {
                        ErrorSeverity = ErrorSeverity.Normal,
                        EnglishMessage = "Unable to stop working, as work currently is not in progress",
                        RussianMessage = "Невозможно перестать работать, так как работа в данный момент не ведется"
                    }
                };
            }

            job.IsWorkingNow = false;

            backgroundJobClient.Delete(job.ProduceItemJobId);
            backgroundJobClient.Delete(job.StopWorkingJobId);

            job.ProduceItemJobId = null;
            job.StopWorkingJobId = null;

            await ctx.SaveChangesAsync();

            return new GameResult<Job>
            {
                Result = await ctx.Jobs.FindAsync(job.Id)
            };
        }

        /// <summary>
        /// Directly produces an item and does so continuously until stopped by <see cref="StopWorking"/>.
        /// </summary>
        /// <returns>Job instance after producing the item</returns>
        [HttpPost]
        public async Task<GameResult<Job>> ProduceItemContinuously([FromBody] string json, [FromServices] BackgroundJobClient backgroundJobClient)
        {
            void Produce(Job job, OutcomeStorage outcomeStorage, WorkerStorage workerStorage, StorageItem storageItem = null)
            {
                var recipe = job.Recipe;
                var ingredients = job.Recipe.Ingredients;

                foreach (var ingredient in ingredients)
                {
                    var item = workerStorage.Items.Single(storageItem => storageItem.Item == ingredient.Item);
                    item.Quantity -= ingredient.Quantity;
                    if (item.Quantity == 0)
                        ctx.StorageItems.Remove(item);
                }

                if (storageItem == null)
                {
                    storageItem = new StorageItem
                    {
                        Item = recipe.ResultItem,
                        Quantity = recipe.ResultItemQuantity
                    };

                    outcomeStorage.Items.Add(storageItem);
                }
                else
                {
                    storageItem.Quantity += recipe.ResultItemQuantity;
                }

                job.ItemsAmountMadeThisWeek += recipe.ResultItemQuantity;
            }

            var pars = new
            {
                jobId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Job job = await ctx.Jobs.FindAsync(d.jobId);
            if (job == null) return JobDoesNotExist(d.jobId);

            //just in case the work is already stopped but the method is called
            if (job.IsWorkingNow.HasValue && job.IsWorkingNow.Value == false)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Critical,
                    EnglishMessage = $"{nameof(ProduceItemContinuously)} is called but {job.IsWorkingNow} is false",
                    RussianMessage = $"{nameof(ProduceItemContinuously)} вызван, но {job.IsWorkingNow} - false"
                };
            }

            var outcomeStorage = job.Company.OutcomeStorage;
            var workerStorage = job.Company.WorkerStorage;

            if (outcomeStorage.Items == null)
            {
                outcomeStorage.Items = new List<StorageItem>();
            }
            if (workerStorage.Items == null)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Unable to produce item due to lack of ingredients",
                    RussianMessage = "Невозможно произвести предмет ввиду отсутствия ингредиентов"
                };
            }

            var currentSpaceOccupied = outcomeStorage.Items.Sum(storageItem => storageItem.Quantity * storageItem.Item.Space);
            if (outcomeStorage.Capacity < currentSpaceOccupied + (job.Recipe.ResultItemQuantity * job.Recipe.ResultItem.Space))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Unable to work, as worker storage is full",
                    RussianMessage = "Невозможно работать, так как рабочий склад полон"
                };
            }

            //does a storage contain enough ingredients to produce the item?
            foreach (var ingredient in job.Recipe.Ingredients)
            {
                var ingredientAmountNeeded = ingredient.Quantity;
                var ingredientAmount = workerStorage.Items.Where(storageItem => storageItem.Item == ingredient.Item)
                                                          .Sum(storageItem => storageItem.Quantity);

                if (ingredientAmount < ingredientAmountNeeded)
                {
                    return new LocalizedError
                    {
                        ErrorSeverity = ErrorSeverity.Normal,
                        EnglishMessage = "Unable to produce item due to lack of ingredients",
                        RussianMessage = "Невозможно произвести предмет ввиду отсутствия ингредиентов"
                    };
                }
            }

            //producing
            var existingStorageItem = outcomeStorage.Items.FirstOrDefault(i => i.Item.Id == job.Recipe.ResultItem.Id);
            if (existingStorageItem != null)
            {
                Produce(job, outcomeStorage, workerStorage, existingStorageItem);
            }
            else
            {
                Produce(job, outcomeStorage, workerStorage);
            }

            var time = TimeSpan.FromSeconds((double)(job.Recipe.LeadTimeInSeconds * job.SpeedModifier));

            var produceItemJobParams = new ProduceItemJobParams
            {
                JobId = job.Id
            };
            string produceItemJobId = backgroundJobClient.Schedule<ProduceItemJob>(j => j.Execute(produceItemJobParams), delay: time);

            job.ProduceItemJobId = produceItemJobId;

            await ctx.SaveChangesAsync();

            return await ctx.Jobs.FindAsync(d.jobId);
        }


        [NonAction]
        private LocalizedError JobDoesNotExist(long id)
        {
            return Errors.DoesNotExist(id, modelLocalization);
        }
    }
}
