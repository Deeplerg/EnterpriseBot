using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Game.Essences;
using EnterpriseBot.Api.Game.Localization;
using EnterpriseBot.Api.Models.ApiCreationParams.Essences;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.Contexts;
using EnterpriseBot.Api.Models.ModelCreationParams.Essences;
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

namespace EnterpriseBot.Api.Areas.Essences.Controllers
{
    [Area(nameof(Essences))]
    public class PlayerController : Controller,
                                    IGameController<Player, PlayerApiCreationParams>
    {
        private readonly ApplicationContext ctx;
        private readonly ILogger<PlayerController> logger;

        private readonly GameSettings gameSettings;

        private readonly LocalizationSettings localization;
        private readonly LocalizationSetting modelLocalization;

        public PlayerController(ApplicationContext dbContext,
                                ILogger<PlayerController> logger,
                                IOptionsSnapshot<GameSettings> gameOptionsAccessor)
        {
            this.ctx = dbContext;
            this.logger = logger;

            this.gameSettings = gameOptionsAccessor.Value;

            this.localization = this.gameSettings.Localization;
            this.modelLocalization = this.localization.Essences.Player;
        }

        ///<inheritdoc/>
        public async Task<GameResult<Player>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            var model = await ctx.Players.FindAsync(id);

            return model;
        }

        public async Task<GameResult<Player>> Create([FromBody] PlayerApiCreationParams pars)
        {
            var creationResult = Player.Create(new PlayerCreationParams
            {
                Name = pars.Name,
                RawPassword = pars.RawPassword
            }, gameSettings);
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.Players.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }


        public async Task<GameResult<bool>> HasPermission([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                permission = default(CompanyJobPermissions),
                companyId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var player = await ctx.Players.FindAsync(d.modelId);
            if (player == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var company = await ctx.Companies.FindAsync(d.companyId);
            if (company == null) return Errors.DoesNotExist(d.companyId, localization.Business.Company.Company);

            var actionResult = player.HasPermission(d.permission, company);

            return actionResult;
        }

        public async Task<GameResult<string>> SetName([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                newName = default(string)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var player = await ctx.Players.FindAsync(d.modelId);
            if (player == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = player.SetName(d.newName, gameSettings);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<StringLocalization>> EditAbout([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                newAbout = default(string),
                language = default(LocalizationLanguage)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var player = await ctx.Players.FindAsync(d.modelId);
            if (player == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = player.EditAbout(d.newAbout, d.language, gameSettings);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<StringLocalization>> EditStatus([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                newStatus = default(string),
                language = default(LocalizationLanguage)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var player = await ctx.Players.FindAsync(d.modelId);
            if (player == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = player.EditStatus(d.newStatus, d.language, gameSettings);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<EmptyGameResult> ChangePassword([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                newPassword = default(string)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var player = await ctx.Players.FindAsync(d.modelId);
            if (player == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = player.ChangePassword(d.newPassword, gameSettings);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<bool>> VerifyPassword([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                password = default(string)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var player = await ctx.Players.FindAsync(d.modelId);
            if (player == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = player.VerifyPassword(d.password);

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<long>> LinkVk([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                vkId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var player = await ctx.Players.FindAsync(d.modelId);
            if (player == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = player.LinkVk(d.vkId);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<EmptyGameResult> UnlinkVk([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var player = await ctx.Players.FindAsync(d.modelId);
            if (player == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = player.UnlinkVk();
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<IEnumerable<Player>>> SearchByName([FromBody] string json)
        {
            var pars = new
            {
                name = default(string)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            string name = d.name.ToLower();

            var players = await ctx.Players
                                   .Where(player => player.Name.ToLower().Contains(name))
                                   .ToListAsync();

            return players;
        }

        public async Task<GameResult<Player>> GetByPlatform([FromBody] string json)
        {
            var pars = new
            {
                platform = default(BotPlatform),
                id = default(string)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            switch (d.platform)
            {
                case BotPlatform.VK:
                    {
                        bool parsedSuccessfully = long.TryParse(d.id, out long id);

                        if (parsedSuccessfully)
                        {
                            return await ctx.Players
                                            .SingleOrDefaultAsync(player => player.VkConnected
                                                                         && player.VkId == id);
                        }
                        else
                        {
                            return null;
                        }
                    }

                default:
                    return Errors.UnknownEnumValue(d.platform);
            }
        }
    }
}
