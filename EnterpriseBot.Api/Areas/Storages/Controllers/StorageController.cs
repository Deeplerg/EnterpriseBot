using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Game.Crafting;
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
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Areas.Storages.Controllers
{
    [Area(nameof(Storages))]
    public class StorageController : Controller,
                                     IGameController<Storage, StorageApiCreationParams>
    {
        private readonly ApplicationContext ctx;
        private readonly ILogger<StorageController> logger;

        private readonly GameSettings gameSettings;

        private readonly LocalizationSettings localization;
        private readonly LocalizationSetting modelLocalization;

        public StorageController(ApplicationContext dbContext,
                                 ILogger<StorageController> logger,
                                 IOptionsSnapshot<GameSettings> gameOptionsAccessor)
        {
            this.ctx = dbContext;
            this.logger = logger;

            this.gameSettings = gameOptionsAccessor.Value;

            this.localization = this.gameSettings.Localization;
            this.modelLocalization = this.localization.Storages.Storage;
        }

        ///<inheritdoc/>
        public async Task<GameResult<Storage>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            var model = await ctx.Storages.FindAsync(id);

            return model;
        }

        public async Task<GameResult<Storage>> Create([FromBody] StorageApiCreationParams pars)
        {
            var creationResult = Storage.Create(new StorageCreationParams
            {
                Capacity = pars.Capacity
            });
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.Storages.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }


        public async Task<GameResult<int>> AddItem([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                itemId = default(long),
                quantity = default(int)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var storage = await ctx.Storages.FindAsync(d.modelId);
            if (storage == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var item = await ctx.Items.FindAsync(d.itemId);
            if (item == null) return Errors.DoesNotExist(d.itemId, localization.Crafting.Item);


            var actionResult = storage.Add(item, d.quantity);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<int>> RemoveItem([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                itemId = default(long),
                quantity = default(int)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var storage = await ctx.Storages.FindAsync(d.modelId);
            if (storage == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var item = await ctx.Items.FindAsync(d.itemId);
            if (item == null) return Errors.DoesNotExist(d.itemId, localization.Crafting.Item);


            var actionResult = storage.Remove(item, d.quantity);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<bool>> ContainsItem([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                itemId = default(long),
                quantity = default(int)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var storage = await ctx.Storages.FindAsync(d.modelId);
            if (storage == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var item = await ctx.Items.FindAsync(d.itemId);
            if (item == null) return Errors.DoesNotExist(d.itemId, localization.Crafting.Item);


            return storage.Contains(item, d.quantity);
        }
         
        public async Task<GameResult<int>> TransferItemToStorage([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                receivingStorageId = default(long),
                itemId = default(long),
                quantity = default(int)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var storage = await ctx.Storages.FindAsync(d.modelId);
            if (storage == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var receivingStorage = await ctx.Storages.FindAsync(d.receivingStorageId);
            if (receivingStorage == null) return Errors.DoesNotExist(d.receivingStorageId, modelLocalization);

            var item = await ctx.Items.FindAsync(d.itemId);
            if (item == null) return Errors.DoesNotExist(d.itemId, localization.Crafting.Item);


            var actionResult = storage.TransferTo(receivingStorage, item, d.quantity);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<int>> TransferEverythingToStorage([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                receivingStorageId = default(long),
                itemTypeToTransferId = default(long?)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var storage = await ctx.Storages.FindAsync(d.modelId);
            if (storage == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var receivingStorage = await ctx.Storages.FindAsync(d.receivingStorageId);
            if (receivingStorage == null) return Errors.DoesNotExist(d.receivingStorageId, modelLocalization);

            var item = await ctx.Items.FindAsync(d.itemTypeToTransferId);


            var actionResult = storage.TransferEverythingTo(receivingStorage, item);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<StorageItem>> GetItem([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                itemId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var storage = await ctx.Storages.FindAsync(d.modelId);
            if (storage == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var item = await ctx.Items.FindAsync(d.itemId);
            if (item == null) return Errors.DoesNotExist(d.itemId, localization.Crafting.Item);


            return storage.GetItem(item);
        }

        public async Task<GameResult<decimal>> AddCapacity([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                amount = default(decimal)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var storage = await ctx.Storages.FindAsync(d.modelId);
            if (storage == null) return Errors.DoesNotExist(d.modelId, modelLocalization);


            var actionResult = storage.AddCapacity(d.amount);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }
    }
}
