using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Game.Localization;
using EnterpriseBot.Api.Models.ApiCreationParams.Localization;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.Contexts;
using EnterpriseBot.Api.Models.ModelCreationParams.Localization;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Areas.Localization.Controllers
{
    [Area(nameof(Localization))]
    public class StringLocalizationController : Controller,
                                                IGameController<StringLocalization, StringLocalizationApiCreationParams>
    {
        private readonly ApplicationContext ctx;
        private readonly ILogger<StringLocalizationController> logger;

        private readonly GameSettings gameSettings;

        private readonly LocalizationSettings localization;
        private readonly LocalizationSetting modelLocalization;

        public StringLocalizationController(ApplicationContext dbContext,
                                            ILogger<StringLocalizationController> logger,
                                            IOptionsSnapshot<GameSettings> gameOptionsAccessor)
        {
            this.ctx = dbContext;
            this.logger = logger;

            this.gameSettings = gameOptionsAccessor.Value;

            this.localization = this.gameSettings.Localization;
            this.modelLocalization = this.localization.Localization.StringLocalization;
        }

        public async Task<GameResult<StringLocalization>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            var model = await ctx.StringLocalizations.FindAsync(id);

            return model;
        }

        public async Task<GameResult<StringLocalization>> Create([FromBody] StringLocalizationApiCreationParams pars)
        {
            var creationResult = StringLocalization.Create(new StringLocalizationCreationParams
            {
                Text = pars.Text,
                Language = pars.Language
            });
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.StringLocalizations.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }

        public async Task<GameResult<string>> SetText([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                newText = default(string)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var stringLocalization = await ctx.StringLocalizations.FindAsync(d.modelId);
            if (stringLocalization == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = stringLocalization.SetText(d.newText);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<LocalizationLanguage>> SetLanguage([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                newLanguage = default(LocalizationLanguage)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var stringLocalization = await ctx.StringLocalizations.FindAsync(d.modelId);
            if (stringLocalization == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = stringLocalization.SetLanguage(d.newLanguage);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }
    }
}
