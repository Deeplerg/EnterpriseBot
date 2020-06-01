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
    public class PersonalStorageController : Controller, IGameController<PersonalStorage>
    {
        private readonly ApplicationContext ctx;
        private readonly GameplaySettings gameplaySettings;
        private readonly LocalizationSettings localizationSettings;

        private readonly LocalizationSetting modelLocalization;

        public PersonalStorageController(ApplicationContext context,
                                         IOptions<GameplaySettings> gameplayOptions,
                                         IOptions<LocalizationSettings> localizationOptions)
        {
            this.ctx = context;
            this.gameplaySettings = gameplayOptions.Value;
            this.localizationSettings = localizationOptions.Value;

            modelLocalization = localizationSettings.Storages.PersonalStorage;
        }

        /// <inheritdoc/>
        [HttpPost]
        public async Task<GameResult<PersonalStorage>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            PersonalStorage personalStorage = await ctx.PersonalStorages.FindAsync(id);
            //if (personalStorage == null) return PersonalStorageDoesNotExist(id);

            return personalStorage;
        }

        //[HttpPost]
        //public async Task<GameResult<PersonalStorage>> Create([FromBody] PersonalStorageCreationParams creationParams)
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
                personalStorageId = default(long),
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

            PersonalStorage personalStorage = await ctx.PersonalStorages.FindAsync(d.personalStorageId);
            if (personalStorage == null) return PersonalStorageDoesNotExist(d.personalStorageId);

            Item item = await ctx.Items.FindAsync(d.itemId);
            if (item == null) return Errors.DoesNotExist(d.itemId, localizationSettings.Crafting.Item);

            if (personalStorage.Items == null)
                personalStorage.Items = new List<StorageItem>();

            var currentSpaceOccupied = personalStorage.Items.Sum(storageItem => storageItem.Quantity * storageItem.Item.Space);
            if (personalStorage.Capacity < currentSpaceOccupied + (d.quantity * item.Space))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The storage doesn't have enough space",
                    RussianMessage = "На складе недостаточно места"
                };
            }

            personalStorage.Items.Add(new StorageItem
            {
                Item = item,
                Quantity = d.quantity
            });

            await ctx.SaveChangesAsync();

            return (await ctx.PersonalStorages.FindAsync(d.personalStorageId)).Items;
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
                personalStorageId = default(long),
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

            PersonalStorage personalStorage = await ctx.PersonalStorages.FindAsync(d.personalStorageId);
            if (personalStorage == null) return PersonalStorageDoesNotExist(d.personalStorageId);

            Item item = await ctx.Items.FindAsync(d.itemId);
            if (item == null) return Errors.DoesNotExist(d.itemId, localizationSettings.Crafting.Item);

            if (personalStorage.Items == null || !personalStorage.Items.Any(storageItem => storageItem.Item == item))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The storage does not have this item",
                    RussianMessage = "На складе нет такого предмета"
                };
            }

            StorageItem itemInStorage = personalStorage.Items.Single(storageItem => storageItem.Item == item);

            if (itemInStorage.Quantity <= d.quantity)
            {
                ctx.StorageItems.Remove(itemInStorage);
            }
            else
            {
                itemInStorage.Quantity -= d.quantity;
            }

            await ctx.SaveChangesAsync();

            return (await ctx.PersonalStorages.FindAsync(d.personalStorageId)).Items;
        }

        /// <summary>
        /// Removes all items from a storage
        /// </summary>
        [HttpPost]
        public async Task<EmptyGameResult> Clear([FromBody] string json)
        {
            var pars = new
            {
                personalStorageId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            PersonalStorage personalStorage = await ctx.PersonalStorages.FindAsync(d.personalStorageId);
            if (personalStorage == null) return PersonalStorageDoesNotExist(d.personalStorageId);

            personalStorage.Items.Clear();

            await ctx.SaveChangesAsync();

            return new EmptyGameResult();
        }


        [NonAction]
        private LocalizedError PersonalStorageDoesNotExist(long id)
        {
            return Errors.DoesNotExist(id, localizationSettings.Storages.PersonalStorage);
        }
    }
}
