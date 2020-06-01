using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Models.Common.Crafting;
using EnterpriseBot.Api.Models.Common.Storages;
using EnterpriseBot.Api.Models.Contexts;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings.GameplaySettings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Areas.Storages
{
    [Area(nameof(Storages))]
    public class WorkerStorageController : Controller, IGameController<WorkerStorage>
    {
        private readonly ApplicationContext ctx;
        private readonly GameplaySettings gameplaySettings;
        private readonly LocalizationSettings localizationSettings;

        private readonly LocalizationSetting modelLocalization;

        public WorkerStorageController(ApplicationContext context,
                                         IOptions<GameplaySettings> gameplayOptions,
                                         IOptions<LocalizationSettings> localizationOptions)
        {
            this.ctx = context;
            this.gameplaySettings = gameplayOptions.Value;
            this.localizationSettings = localizationOptions.Value;

            modelLocalization = localizationSettings.Storages.WorkerStorage;
        }

        /// <inheritdoc/>
        public async Task<GameResult<WorkerStorage>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            WorkerStorage workerStorage = await ctx.WorkerStorages.FindAsync(id);
            //if (workerStorage == null) return WorkerStorageDoesNotExist(id);

            return workerStorage;
        }

        //[HttpPost]
        //public async Task<GameResult<WorkerStorage>> Create([FromBody] WorkerStorageCreationParams creationParams)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Adds an item to the storage
        /// </summary>
        /// <returns>Storage items</returns>
        [HttpPost]
        public async Task<GameResult<List<StorageItem>>> AddItem([FromBody] string json)
        {
            var pars = new
            {
                workerStorageId = default(long),
                itemId = default(long),
                quantity = default(int)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            if (d.quantity <= 0)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Item quantity can not be lower than or equal to 0",
                    RussianMessage = "Количество предметов не может быть меньше или равно 0"
                };
            }

            WorkerStorage workerStorage = await ctx.WorkerStorages.FindAsync(d.workerStorageId);
            if (workerStorage == null) return WorkerStorageDoesNotExist(d.workerStorageId);

            Item item = await ctx.Items.FindAsync(d.itemId);
            if (item == null) return Errors.DoesNotExist(d.itemId, localizationSettings.Crafting.Item);

            if (workerStorage.Items == null)
                workerStorage.Items = new List<StorageItem>();

            var currentSpaceOccupied = workerStorage.Items.Sum(storageItem => storageItem.Quantity * storageItem.Item.Space);
            if (workerStorage.Capacity < currentSpaceOccupied + (d.quantity * item.Space))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The storage doesn't have enough space",
                    RussianMessage = "На складе недостаточно места"
                };
            }

            workerStorage.Items.Add(new StorageItem
            {
                Item = item,
                Quantity = d.quantity
            });

            await ctx.SaveChangesAsync();

            return (await ctx.WorkerStorages.FindAsync(d.workerStorageId)).Items;
        }

        /// <summary>
        /// Removes an item from the storage
        /// </summary>
        /// <returns>Storage items</returns>
        [HttpPost]
        public async Task<GameResult<List<StorageItem>>> RemoveItem([FromBody] string json)
        {
            var pars = new
            {
                workerStorageId = default(long),
                itemId = default(long),
                quantity = default(int)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            if (d.quantity <= 0)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Item quantity can not be lower than or equal to 0",
                    RussianMessage = "Количество предметов не может быть меньше или равно 0"
                };
            }

            WorkerStorage workerStorage = await ctx.WorkerStorages.FindAsync(d.workerStorageId);
            if (workerStorage == null) return WorkerStorageDoesNotExist(d.workerStorageId);

            Item item = await ctx.Items.FindAsync(d.itemId);
            if (item == null) return Errors.DoesNotExist(d.itemId, localizationSettings.Crafting.Item);

            if (workerStorage.Items == null || !workerStorage.Items.Any(storageItem => storageItem.Item == item))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The storage does not have this item",
                    RussianMessage = "На складе нет такого предмета"
                };
            }

            StorageItem itemInStorage = workerStorage.Items.Single(storageItem => storageItem.Item == item);

            if (itemInStorage.Quantity <= d.quantity)
            {
                ctx.StorageItems.Remove(itemInStorage);
            }
            else
            {
                itemInStorage.Quantity -= d.quantity;
            }

            await ctx.SaveChangesAsync();

            return (await ctx.WorkerStorages.FindAsync(d.workerStorageId)).Items;
        }

        /// <summary>
        /// Moves an item to an outcome storage
        /// </summary>
        /// <returns>Worker storage instance</returns>
        [HttpPost]
        public async Task<GameResult<WorkerStorage>> MoveToOutcomeStorage([FromBody] string json)
        {
            var pars = new
            {
                workerStorageId = default(long),
                outcomeStorageId = default(long),
                itemId = default(long),
                quantity = default(int)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            if (d.quantity <= 0)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Item quantity can not be lower than or equal to 0",
                    RussianMessage = "Количество предметов не может быть меньше или равно 0"
                };
            }

            WorkerStorage workerStorage = await ctx.WorkerStorages.FindAsync(d.workerStorageId);
            if (workerStorage == null) return WorkerStorageDoesNotExist(d.workerStorageId);

            OutcomeStorage outcomeStorage = await ctx.OutcomeStorages.FindAsync(d.outcomeStorageId);
            if (outcomeStorage == null) return Errors.DoesNotExist(d.outcomeStorageId, localizationSettings.Storages.OutcomeStorage);

            Item item = await ctx.Items.FindAsync(d.itemId);
            if (item == null) return Errors.DoesNotExist(d.itemId, localizationSettings.Crafting.Item);

            if (workerStorage.Items == null || !workerStorage.Items.Any(storageItem => storageItem.Item == item))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The storage does not have this item",
                    RussianMessage = "На складе нет такого предмета"
                };
            }

            var currentSpaceOccupied = outcomeStorage.Items.Sum(storageItem => storageItem.Quantity * storageItem.Item.Space);
            if (outcomeStorage.Capacity < currentSpaceOccupied + (d.quantity * item.Space))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The storage doesn't have enough space",
                    RussianMessage = "На складе недостаточно места"
                };
            }


            StorageItem itemInStorage = workerStorage.Items.Single(storageItem => storageItem.Item == item);

            if (itemInStorage.Quantity <= d.quantity)
            {
                ctx.StorageItems.Remove(itemInStorage);
            }
            else
            {
                itemInStorage.Quantity -= d.quantity;
            }


            var itemInDestinationStorage = outcomeStorage.Items.Where(storageItem => storageItem.Item == item).FirstOrDefault();

            if (itemInDestinationStorage == null)
            {
                itemInDestinationStorage = new StorageItem
                {
                    Item = item,
                    Quantity = d.quantity
                };

                outcomeStorage.Items.Add(itemInDestinationStorage);
            }
            else
            {
                itemInDestinationStorage.Quantity += d.quantity;
            }


            await ctx.SaveChangesAsync();

            return await ctx.WorkerStorages.FindAsync(d.workerStorageId);
        }


        [NonAction]
        private LocalizedError WorkerStorageDoesNotExist(long id)
        {
            return Errors.DoesNotExist(id, modelLocalization);
        }
    }
}
