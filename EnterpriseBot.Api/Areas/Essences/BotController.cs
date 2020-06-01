using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Models.Common.Business;
using EnterpriseBot.Api.Models.Common.Essences;
using EnterpriseBot.Api.Models.Contexts;
using EnterpriseBot.Api.Models.ModelCreationParams.Essences;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings.GameplaySettings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using EnterpriseBot.BackgroundJobs.Jobs;
using EnterpriseBot.BackgroundJobs.Params;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using static EnterpriseBot.Api.Utils.Constants;
using static EnterpriseBot.Api.Utils.Miscellaneous;

namespace EnterpriseBot.Api.Areas.Essences
{
    [Area(nameof(Essences))]
    public class BotController : Controller, IGameController<Bot>
    {
        private readonly ApplicationContext ctx;
        private readonly GameplaySettings gameplaySettings;
        private readonly LocalizationSettings localizationSettings;

        private readonly LocalizationSetting modelLocalization;

        private readonly IBackgroundJobClient backgroundJobClient;

        public BotController(ApplicationContext context,
                             IOptions<GameplaySettings> gameplayOptions,
                             IOptions<LocalizationSettings> localizationOptions,
                             IBackgroundJobClient backgroundJobClient)
        {
            this.ctx = context;
            this.gameplaySettings = gameplayOptions.Value;
            this.localizationSettings = localizationOptions.Value;

            modelLocalization = localizationSettings.Essences.Bot;

            this.backgroundJobClient = backgroundJobClient;
        }

        /// <inheritdoc/>
        [HttpPost]
        public async Task<GameResult<Bot>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            Bot bot = await ctx.Bots.FindAsync(id);
            //if (bot == null) return BotDoesNotExist(id);

            return bot;
        }

        ///// <inheritdoc/>
        /// <remarks>Automatically creates a task and occupies the job</remarks>
        [HttpPost]
        public async Task<GameResult<Bot>> Create([FromBody] BotCreationParams cp)
        {
            return await CreateBot(cp, this.backgroundJobClient);
        }

        /// <summary>
        /// Buys a bot and automatically hires it on the specified job
        /// </summary>
        /// <returns>Bot instance after hiring</returns>
        [HttpPost]
        public async Task<GameResult<Bot>> BuyBotAndHire([FromBody] BotCreationParams cp)
        {
            return await CreateBot(cp, backgroundJobClient);
        }

        /// <summary>
        /// Removes the bot
        /// </summary>
        [HttpPost]
        public async Task<EmptyGameResult> Remove([FromBody] string json)
        {
            var pars = new
            {
                botId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Bot bot = await ctx.Bots.FindAsync(d.botId);
            if (bot == null) return BotDoesNotExist(d.botId);

            Job job = bot.Job;

            string salaryPayerJobId = job.SalaryPayerJobId;

            job.IsOccupied = false;
            job.IsBot = null;
            job.ItemsAmountMadeThisWeek = 0;
            job.Salary = gameplaySettings.Job.MinSalary;

            backgroundJobClient.Delete(salaryPayerJobId);
            job.SalaryPayerJobId = null;

            ctx.Bots.Remove(bot);

            await ctx.SaveChangesAsync();

            return new EmptyGameResult();
        }


        [NonAction]
        private async Task<GameResult<Bot>> CreateBot(BotCreationParams cp, IBackgroundJobClient backgroundJobClient)
        {
            UserInputRequirements req = localizationSettings.UserInputRequirements;

            if (!CheckName(cp.Name))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Bot name has not passed verification. " + string.Format(req.Name.English, NameMaxLength),
                    RussianMessage = "Имя бота не прошло проверку. " + string.Format(req.Name.Russian, NameMaxLength)
                };
            }

            Job job = await ctx.Jobs.FindAsync(cp.JobId);
            if (job == null) return Errors.DoesNotExist(cp.JobId, localizationSettings.Business.Job);

            if (job.IsOccupied)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The job is already occupied",
                    RussianMessage = "Работа уже занята"
                };
            }

            if (job.Company.CompanyUnits < gameplaySettings.Prices.Bot.DefaultPrice)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The company doesn't have enough units to buy the bot",
                    RussianMessage = "У компании недостаточно юнитов, чтобы купить бота"
                };
            }

            //can be optimized by removing "ToList", which is not recommended (won't generate request with Trim and string.Equals)
            if ((await ctx.Bots.ToListAsync()).Any(bot => CompareNames(bot.Name, cp.Name)))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "A bot with the same name already exists",
                    RussianMessage = "Бот с таким именем уже существует"
                };
            }

            Bot createdBot = new Bot
            {
                Job = job,
                Name = cp.Name
            };

            await ctx.Bots.AddAsync(createdBot);

            job.IsOccupied = true;
            job.IsBot = true;
            job.ItemsAmountMadeThisWeek = 0;
            job.Salary = gameplaySettings.Job.BotMaintenanceSalary;

            await ctx.SaveChangesAsync();

            var salaryPayerJobParams = new SalaryPayerJobParams
            {
                JobId = job.Id
            };
            string salaryPayerJobId = backgroundJobClient.Schedule<SalaryPayerJob>(job => job.Execute(salaryPayerJobParams), TimeSpan.FromDays(7));

            job.SalaryPayerJobId = salaryPayerJobId;

            await ctx.SaveChangesAsync();

            return await ctx.Bots.FindAsync(createdBot.Id);
        }

        [NonAction]
        private LocalizedError BotDoesNotExist(long id)
        {
            return Errors.DoesNotExist(id, modelLocalization);
        }
    }
}
