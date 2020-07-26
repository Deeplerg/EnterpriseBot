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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Areas.Localization.Controllers
{
    [Area(nameof(Localization))]
    public class LocalizedStringController : Controller,
                                             IGameController<LocalizedString, LocalizedStringApiCreationParams>
    {
        private readonly ApplicationContext ctx;
        private readonly ILogger<LocalizedStringController> logger;

        private readonly GameSettings gameSettings;

        private readonly LocalizationSettings localization;
        private readonly LocalizationSetting modelLocalization;

        public LocalizedStringController(ApplicationContext dbContext,
                                         ILogger<LocalizedStringController> logger,
                                         IOptionsSnapshot<GameSettings> gameOptionsAccessor)
        {
            this.ctx = dbContext;
            this.logger = logger;

            this.gameSettings = gameOptionsAccessor.Value;

            this.localization = this.gameSettings.Localization;
            this.modelLocalization = this.localization.Localization.LocalizedString;
        }

        public async Task<GameResult<LocalizedString>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            var model = await ctx.LocalizedStrings.FindAsync(id);

            return model;
        }

        public async Task<GameResult<LocalizedString>> Create([FromBody] LocalizedStringApiCreationParams pars)
        {
            var localizations = new List<StringLocalization>();

            if (pars.LocalizedStringIds?.Any() is true)
            {
                localizations = await ctx.StringLocalizations
                                         .Where(str =>
                                                pars.LocalizedStringIds.Any(id => str.Id == id))
                                         .ToListAsync();
            }

            var creationResult = LocalizedString.Create(new LocalizedStringCreationParams
            {
                Localizations = localizations
            });
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }

        public async Task<GameResult<StringLocalization>> AddLocalization([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                localizationParams = default(StringLocalizationApiCreationParams)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var localizedString = await ctx.LocalizedStrings.FindAsync(d.modelId);
            if (localizedString == null) return Errors.DoesNotExist(d.modelId, modelLocalization);


            var creationParams = new StringLocalizationCreationParams
            {
                Text = d.localizationParams.Text,
                Language = d.localizationParams.Language
            };

            var actionResult = localizedString.AddLocalization(creationParams);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<StringLocalization>> Edit([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                newText = default(string),
                language = default(LocalizationLanguage)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var localizedString = await ctx.LocalizedStrings.FindAsync(d.modelId);
            if (localizedString == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = localizedString.Edit(d.newText, d.language);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<StringLocalization>> GetLocalization([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                language = default(LocalizationLanguage)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var localizedString = await ctx.LocalizedStrings.FindAsync(d.modelId);
            if (localizedString == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = localizedString.GetLocalization(d.language);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<bool>> IsLocalizationPresent([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                language = default(LocalizationLanguage)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var localizedString = await ctx.LocalizedStrings.FindAsync(d.modelId);
            if (localizedString == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = localizedString.IsLocalizationPresent(d.language);

            await ctx.SaveChangesAsync();

            return actionResult;
        }
    }
}
