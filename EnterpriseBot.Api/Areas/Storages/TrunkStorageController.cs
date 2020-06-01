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
    public class TrunkStorageController : Controller, IGameController<TrunkStorage>
    {
        private readonly ApplicationContext ctx;
        private readonly GameplaySettings gameplaySettings;
        private readonly LocalizationSettings localizationSettings;

        private readonly LocalizationSetting modelLocalization;

        public TrunkStorageController(ApplicationContext context,
                                         IOptions<GameplaySettings> gameplayOptions,
                                         IOptions<LocalizationSettings> localizationOptions)
        {
            this.ctx = context;
            this.gameplaySettings = gameplayOptions.Value;
            this.localizationSettings = localizationOptions.Value;

            modelLocalization = localizationSettings.Storages.TrunkStorage;
        }

        /// <inheritdoc/>
        [HttpPost]
        public async Task<GameResult<TrunkStorage>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            TrunkStorage trunkStorage = await ctx.TrunkStorages.FindAsync(id);
            //if (trunkStorage == null) return TrunkStorageDoesNotExist(id);

            return trunkStorage;
        }

        //[HttpPost]
        //public async Task<GameResult<TrunkStorage>> Create([FromBody] TrunkStorageCreationParams creationParams)
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
                trunkStorageId = default(long),
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

            TrunkStorage trunkStorage = await ctx.TrunkStorages.FindAsync(d.trunkStorageId);
            if (trunkStorage == null) return TrunkStorageDoesNotExist(d.trunkStorageId);

            Item item = await ctx.Items.FindAsync(d.itemId);
            if (item == null) return Errors.DoesNotExist(d.itemId, localizationSettings.Crafting.Item);

            if (trunkStorage.Items == null)
                trunkStorage.Items = new List<StorageItem>();

            var currentSpaceOccupied = trunkStorage.Items.Sum(storageItem => storageItem.Quantity * storageItem.Item.Space);
            if (trunkStorage.Capacity < currentSpaceOccupied + (d.quantity * item.Space))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The storage doesn't have enough space",
                    RussianMessage = "На складе недостаточно места"
                };
            }

            trunkStorage.Items.Add(new StorageItem
            {
                Item = item,
                Quantity = d.quantity
            });

            await ctx.SaveChangesAsync();

            return (await ctx.TrunkStorages.FindAsync(d.trunkStorageId)).Items;
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
                trunkStorageId = default(long),
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

            TrunkStorage trunkStorage = await ctx.TrunkStorages.FindAsync(d.trunkStorageId);
            if (trunkStorage == null) return TrunkStorageDoesNotExist(d.trunkStorageId);

            Item item = await ctx.Items.FindAsync(d.itemId);
            if (item == null) return Errors.DoesNotExist(d.itemId, localizationSettings.Crafting.Item);

            if (trunkStorage.Items == null || !trunkStorage.Items.Any(storageItem => storageItem.Item == item))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The storage does not have this item",
                    RussianMessage = "На складе нет такого предмета"
                };
            }

            StorageItem itemInStorage = trunkStorage.Items.Single(storageItem => storageItem.Item == item);

            if (itemInStorage.Quantity <= d.quantity)
            {
                ctx.StorageItems.Remove(itemInStorage);
            }
            else
            {
                itemInStorage.Quantity -= d.quantity;
            }

            await ctx.SaveChangesAsync();

            return (await ctx.TrunkStorages.FindAsync(d.trunkStorageId)).Items;
        }

        /// <summary>
        /// Moves an item to an income storage
        /// </summary>
        /// <returns>Trunk storage instance</returns>
        [HttpPost]
        public async Task<GameResult<TrunkStorage>> MoveToIncomeStorage([FromBody] string json)
        {
            var pars = new
            {
                trunkStorageId = default(long),
                incomeStorageId = default(long),
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

            TrunkStorage trunkStorage = await ctx.TrunkStorages.FindAsync(d.trunkStorageId);
            if (trunkStorage == null) return TrunkStorageDoesNotExist(d.trunkStorageId);

            IncomeStorage incomeStorage = await ctx.IncomeStorages.FindAsync(d.incomeStorageId);
            if (incomeStorage == null) return Errors.DoesNotExist(d.incomeStorageId, localizationSettings.Storages.IncomeStorage);

            Item item = await ctx.Items.FindAsync(d.itemId);
            if (item == null) return Errors.DoesNotExist(d.itemId, localizationSettings.Crafting.Item);

            if (trunkStorage.Items == null || !trunkStorage.Items.Any(storageItem => storageItem.Item == item))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The storage does not have this item",
                    RussianMessage = "На складе нет такого предмета"
                };
            }

            var currentSpaceOccupied = incomeStorage.Items.Sum(storageItem => storageItem.Quantity * storageItem.Item.Space);
            if (incomeStorage.Capacity < currentSpaceOccupied + (d.quantity * item.Space))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The storage doesn't have enough space",
                    RussianMessage = "На складе недостаточно места"
                };
            }


            StorageItem itemInStorage = trunkStorage.Items.Single(storageItem => storageItem.Item == item);

            if (itemInStorage.Quantity <= d.quantity)
            {
                ctx.StorageItems.Remove(itemInStorage);
            }
            else
            {
                itemInStorage.Quantity -= d.quantity;
            }


            var itemInDestinationStorage = incomeStorage.Items.Where(storageItem => storageItem.Item == item).FirstOrDefault();

            if (itemInDestinationStorage == null)
            {
                itemInDestinationStorage = new StorageItem
                {
                    Item = item,
                    Quantity = d.quantity
                };

                incomeStorage.Items.Add(itemInDestinationStorage);
            }
            else
            {
                itemInDestinationStorage.Quantity += d.quantity;
            }


            await ctx.SaveChangesAsync();

            return await ctx.TrunkStorages.FindAsync(d.trunkStorageId);
        }

        /// <summary>
        /// Removes all items from a storage
        /// </summary>
        [HttpPost]
        public async Task<EmptyGameResult> Clear([FromBody] string json)
        {
            var pars = new
            {
                trunkStorageId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            TrunkStorage trunkStorage = await ctx.TrunkStorages.FindAsync(d.trunkStorageId);
            if (trunkStorage == null) return TrunkStorageDoesNotExist(d.trunkStorageId);

            trunkStorage.Items.Clear();

            await ctx.SaveChangesAsync();

            return new EmptyGameResult();
        }


        [NonAction]
        private LocalizedError TrunkStorageDoesNotExist(long id)
        {
            return Errors.DoesNotExist(id, modelLocalization);
        }
    }
}
