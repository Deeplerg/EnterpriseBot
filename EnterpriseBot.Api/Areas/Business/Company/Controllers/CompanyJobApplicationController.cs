using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Attributes;
using EnterpriseBot.Api.Game.Business.Company;
using EnterpriseBot.Api.Models.ApiCreationParams.Business;
using EnterpriseBot.Api.Models.Contexts;
using EnterpriseBot.Api.Models.ModelCreationParams.Business;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Areas.Business.Company.Controllers
{
    [Area(nameof(Business))]
    [SubArea(nameof(Business.Company))]
    public class CompanyJobApplicationController : Controller,
                                                   IGameController<CompanyJobApplication, CompanyJobApplicationApiCreationParams>
    {
        private readonly ApplicationContext ctx;
        private readonly ILogger<CompanyJobApplicationController> logger;

        private readonly GameSettings gameSettings;

        private readonly LocalizationSettings localization;
        private readonly LocalizationSetting modelLocalization;

        public CompanyJobApplicationController(ApplicationContext dbContext,
                                               ILogger<CompanyJobApplicationController> logger,
                                               IOptionsSnapshot<GameSettings> gameOptionsAccessor)
        {
            this.ctx = dbContext;
            this.logger = logger;

            this.gameSettings = gameOptionsAccessor.Value;

            this.localization = this.gameSettings.Localization;
            this.modelLocalization = this.localization.Business.Company.CompanyJobApplication;
        }

        ///<inheritdoc/>
        public async Task<GameResult<CompanyJobApplication>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            var model = await ctx.CompanyJobApplications.FindAsync(id);

            return model;
        }

        public async Task<GameResult<CompanyJobApplication>> Create([FromBody] CompanyJobApplicationApiCreationParams pars)
        {
            var applicant = await ctx.Players.FindAsync(pars.ApplicantPlayerId);
            if (applicant == null) return Errors.DoesNotExist(applicant.Id, localization.Essences.Player);

            var job = await ctx.CompanyJobs.FindAsync(pars.CompanyJobId);
            if (job == null) return Errors.DoesNotExist(job.Id, modelLocalization);

            var creationResult = CompanyJobApplication.Create(new CompanyJobApplicationCreationParams
            {
                Applicant = applicant,
                Job = job,
                Resume = pars.Resume
            }, gameSettings);
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.CompanyJobApplications.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }


        public async Task<EmptyGameResult> Decline([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var companyJobApplication = await ctx.CompanyJobApplications.FindAsync(d.modelId);
            if (companyJobApplication == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            ctx.CompanyJobApplications.Remove(companyJobApplication);
            await ctx.SaveChangesAsync();

            return new EmptyGameResult();
        }
    }
}
