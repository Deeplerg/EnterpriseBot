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
    public class StorageItemController : Controller,
                                         IGameController<StorageItem, StorageItemApiCreationParams>
    {
        private readonly ApplicationContext ctx;
        private readonly ILogger<StorageItemController> logger;

        private readonly GameSettings gameSettings;

        private readonly LocalizationSettings localization;
        private readonly LocalizationSetting modelLocalization;

        public StorageItemController(ApplicationContext dbContext,
                                     ILogger<StorageItemController> logger,
                                     IOptionsSnapshot<GameSettings> gameOptionsAccessor)
        {
            this.ctx = dbContext;
            this.logger = logger;

            this.gameSettings = gameOptionsAccessor.Value;

            this.localization = this.gameSettings.Localization;
            this.modelLocalization = this.localization.Storages.StorageItem;
        }

        ///<inheritdoc/>
        public async Task<GameResult<StorageItem>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            var model = await ctx.StorageItems.FindAsync(id);

            return model;
        }

        public async Task<GameResult<StorageItem>> Create([FromBody] StorageItemApiCreationParams pars)
        {
            var item = await ctx.Items.FindAsync(pars.ItemId);
            if (item == null) return Errors.DoesNotExist(pars.ItemId, localization.Crafting.Item);

            var creationResult = StorageItem.Create(new StorageItemCreationParams
            {
                Item = item,
                Quantity = pars.Quantity
            });
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.StorageItems.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }


        public async Task<GameResult<int>> AddQuantity([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                amount = default(int)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var storageItem = await ctx.StorageItems.FindAsync(d.modelId);
            if (storageItem == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = storageItem.AddQuantity(d.amount);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<int>> ReduceQuantity([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                amount = default(int)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var storageItem = await ctx.StorageItems.FindAsync(d.modelId);
            if (storageItem == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = storageItem.ReduceQuantity(d.amount);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }
    }
}
