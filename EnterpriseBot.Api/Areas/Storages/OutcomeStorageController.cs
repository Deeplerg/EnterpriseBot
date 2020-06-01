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
    public class OutcomeStorageController : Controller, IGameController<OutcomeStorage>
    {
        private readonly ApplicationContext ctx;
        private readonly GameplaySettings gameplaySettings;
        private readonly LocalizationSettings localizationSettings;

        private readonly LocalizationSetting modelLocalization;

        public OutcomeStorageController(ApplicationContext context,
                                        IOptions<GameplaySettings> gameplayOptions,
                                        IOptions<LocalizationSettings> localizationOptions)
        {
            this.ctx = context;
            this.gameplaySettings = gameplayOptions.Value;
            this.localizationSettings = localizationOptions.Value;

            modelLocalization = localizationSettings.Storages.OutcomeStorage;
        }

        /// <inheritdoc/>
        [HttpPost]
        public async Task<GameResult<OutcomeStorage>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            OutcomeStorage outcomeStorage = await ctx.OutcomeStorages.FindAsync(id);
            //if (outcomeStorage == null) return OutcomeStorageDoesNotExist(id);

            return outcomeStorage;
        }

        //[HttpPost]
        //public async Task<GameResult<OutcomeStorage>> Create([FromBody] OutcomeStorageCreationParams creationParams)
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

            OutcomeStorage outcomeStorage = await ctx.OutcomeStorages.FindAsync(d.outcomeStorageId);
            if (outcomeStorage == null) return OutcomeStorageDoesNotExist(d.outcomeStorageId);

            Item item = await ctx.Items.FindAsync(d.itemId);
            if (item == null) return Errors.DoesNotExist(d.itemId, localizationSettings.Crafting.Item);

            if (outcomeStorage.Items == null)
                outcomeStorage.Items = new List<StorageItem>();

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

            outcomeStorage.Items.Add(new StorageItem
            {
                Item = item,
                Quantity = d.quantity
            });

            await ctx.SaveChangesAsync();

            return (await ctx.OutcomeStorages.FindAsync(d.outcomeStorageId)).Items;
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

            IncomeStorage outcomeStorage = await ctx.IncomeStorages.FindAsync(d.outcomeStorageId);
            if (outcomeStorage == null) return OutcomeStorageDoesNotExist(d.outcomeStorageId);

            Item item = await ctx.Items.FindAsync(d.itemId);
            if (item == null) return Errors.DoesNotExist(d.itemId, localizationSettings.Crafting.Item);

            if (outcomeStorage.Items == null || !outcomeStorage.Items.Any(storageItem => storageItem.Item == item))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The storage does not have this item",
                    RussianMessage = "На складе нет такого предмета"
                };
            }

            StorageItem itemInStorage = outcomeStorage.Items.Single(storageItem => storageItem.Item == item);

            if (itemInStorage.Quantity <= d.quantity)
            {
                ctx.StorageItems.Remove(itemInStorage);
            }
            else
            {
                itemInStorage.Quantity -= d.quantity;
            }

            await ctx.SaveChangesAsync();

            return (await ctx.OutcomeStorages.FindAsync(d.outcomeStorageId)).Items;
        }

        /// <summary>
        /// Moves an item to a trunk storage
        /// </summary>
        /// <returns>Trunk storage instance</returns>
        [HttpPost]
        public async Task<GameResult<TrunkStorage>> MoveToTrunkStorage([FromBody] string json)
        {
            var pars = new
            {
                outcomeStorageId = default(long),
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

            OutcomeStorage outcomeStorage = await ctx.OutcomeStorages.FindAsync(d.outcomeStorageId);
            if (outcomeStorage == null) return OutcomeStorageDoesNotExist(d.outcomeStorageId);

            TrunkStorage trunkStorage = await ctx.TrunkStorages.FindAsync(d.trunkStorageId);
            if (trunkStorage == null) return Errors.DoesNotExist(d.trunkStorageId, localizationSettings.Storages.TrunkStorage);

            Item item = await ctx.Items.FindAsync(d.itemId);
            if (item == null) return Errors.DoesNotExist(d.itemId, localizationSettings.Crafting.Item);

            if (outcomeStorage.Items == null || !outcomeStorage.Items.Any(storageItem => storageItem.Item == item))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The storage does not have this item",
                    RussianMessage = "На складе нет такого предмета"
                };
            }

            var currentSpaceOccupied = trunkStorage.Items.Sum(storageItem => storageItem.Quantity * storageItem.Item.Space);
            if (trunkStorage.Capacity < currentSpaceOccupied + (d.quantity * item.Space))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The truck body is full",
                    RussianMessage = "Кузов полон"
                };
            }


            StorageItem itemInStorage = outcomeStorage.Items.Single(storageItem => storageItem.Item == item);

            if (itemInStorage.Quantity <= d.quantity)
            {
                ctx.StorageItems.Remove(itemInStorage);
            }
            else
            {
                itemInStorage.Quantity -= d.quantity;
            }


            var itemInDestinationStorage = trunkStorage.Items.Where(storageItem => storageItem.Item == item).FirstOrDefault();

            if (itemInDestinationStorage == null)
            {
                itemInDestinationStorage = new StorageItem
                {
                    Item = item,
                    Quantity = d.quantity
                };

                trunkStorage.Items.Add(itemInDestinationStorage);
            }
            else
            {
                itemInDestinationStorage.Quantity += d.quantity;
            }


            await ctx.SaveChangesAsync();

            return await ctx.TrunkStorages.FindAsync(d.trunkStorageId);
        }


        [NonAction]
        private LocalizedError OutcomeStorageDoesNotExist(long id)
        {
            return Errors.DoesNotExist(id, modelLocalization);
        }
    }
}
