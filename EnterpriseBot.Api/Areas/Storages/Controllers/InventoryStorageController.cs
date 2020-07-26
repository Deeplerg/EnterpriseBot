using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Game.Storages;
using EnterpriseBot.Api.Models.ApiCreationParams.Storages;
using EnterpriseBot.Api.Models.Contexts;
using EnterpriseBot.Api.Models.ModelCreationParams.Storages;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Areas.Storages.Controllers
{
    [Area(nameof(Storages))]
    public class InventoryStorageController : Controller,
                                              IGameController<InventoryStorage, InventoryStorageApiCreationParams>
    {
        private readonly ApplicationContext ctx;
        private readonly ILogger<InventoryStorageController> logger;

        private readonly GameSettings gameSettings;

        private readonly LocalizationSettings localization;
        private readonly LocalizationSetting modelLocalization;

        public InventoryStorageController(ApplicationContext dbContext,
                                          ILogger<InventoryStorageController> logger,
                                          IOptionsSnapshot<GameSettings> gameOptionsAccessor)
        {
            this.ctx = dbContext;
            this.logger = logger;

            this.gameSettings = gameOptionsAccessor.Value;

            this.localization = this.gameSettings.Localization;
            this.modelLocalization = this.localization.Storages.InventoryStorage;
        }

        ///<inheritdoc/>
        public async Task<GameResult<InventoryStorage>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            var model = await ctx.InventoryStorages.FindAsync(id);

            return model;
        }

        public async Task<GameResult<InventoryStorage>> Create([FromBody] InventoryStorageApiCreationParams pars)
        {
            var player = await ctx.Players.FindAsync(pars.OwningPlayerId);
            if (player == null) return Errors.DoesNotExist(pars.OwningPlayerId, localization.Essences.Player);

            var creationResult = InventoryStorage.Create(new InventoryStorageCreationParams
            {
                OwningPlayer = player,
                Capacity = pars.Capacity
            });
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.InventoryStorages.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }


        public async Task<GameResult<decimal>> UpgradeCapacity([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var inventoryStorage = await ctx.InventoryStorages.FindAsync(d.modelId);
            if (inventoryStorage == null) return Errors.DoesNotExist(d.modelId, modelLocalization);


            var actionResult = inventoryStorage.UpgradeCapacity(gameSettings);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }
    }
}
