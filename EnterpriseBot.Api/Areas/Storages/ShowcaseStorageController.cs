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
    public class ShowcaseStorageController : Controller, IGameController<ShowcaseStorage>
    {
        private readonly ApplicationContext ctx;
        private readonly GameplaySettings gameplaySettings;
        private readonly LocalizationSettings localizationSettings;

        private readonly LocalizationSetting modelLocalization;

        public ShowcaseStorageController(ApplicationContext context,
                                         IOptions<GameplaySettings> gameplayOptions,
                                         IOptions<LocalizationSettings> localizationOptions)
        {
            this.ctx = context;
            this.gameplaySettings = gameplayOptions.Value;
            this.localizationSettings = localizationOptions.Value;

            modelLocalization = localizationSettings.Storages.ShowcaseStorage;
        }

        [HttpPost]
        public async Task<GameResult<ShowcaseStorage>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            ShowcaseStorage showcaseStorage = await ctx.ShowcaseStorages.FindAsync(id);
            //if (showcaseStorage == null) return ShowcaseStorageDoesNotExist(id);

            return showcaseStorage;
        }

        //[HttpPost]
        //public async Task<GameResult<ShowcaseStorage>> Create([FromBody] ShowcaseStorageCreationParams creationParams)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Adds an item to the storage
        /// </summary>
        /// <returns>Storage items</returns>
        [HttpPost]
        public async Task<GameResult<List<StorageItemWithPrice>>> AddItem([FromBody] string json)
        {
            var pars = new
            {
                showcaseStorageId = default(long),
                itemId = default(long),
                quantity = default(int),
                price = default(decimal)
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
            if (d.price <= 0)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Price can not be lower than or equal to 0",
                    RussianMessage = "Цена не может быть меньше или равна 0"
                };
            }

            ShowcaseStorage showcaseStorage = await ctx.ShowcaseStorages.FindAsync(d.showcaseStorageId);
            if (showcaseStorage == null) return ShowcaseStorageDoesNotExist(d.showcaseStorageId);

            Item item = await ctx.Items.FindAsync(d.itemId);
            if (item == null) return Errors.DoesNotExist(d.itemId, localizationSettings.Crafting.Item);

            if (showcaseStorage.Items == null)
                showcaseStorage.Items = new List<StorageItemWithPrice>();

            var currentSpaceOccupied = showcaseStorage.Items.Sum(priceItem => priceItem.StorageItem.Quantity * priceItem.StorageItem.Item.Space);
            if (showcaseStorage.Capacity < currentSpaceOccupied + (d.quantity * item.Space))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The showcase is full",
                    RussianMessage = "Витрина полна"
                };
            }

            showcaseStorage.Items.Add(new StorageItemWithPrice
            {
                StorageItem = new StorageItem
                {
                    Item = item,
                    Quantity = d.quantity
                },
                Price = d.price
            });

            await ctx.SaveChangesAsync();

            return (await ctx.ShowcaseStorages.FindAsync(d.showcaseStorageId)).Items;
        }

        /// <summary>
        /// Removes an item from the storage
        /// </summary>
        /// <returns>Storage items</returns>
        [HttpPost]
        public async Task<GameResult<List<StorageItemWithPrice>>> RemoveItem([FromBody] string json)
        {
            var pars = new
            {
                showcaseStorageId = default(long),
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

            ShowcaseStorage showcaseStorage = await ctx.ShowcaseStorages.FindAsync(d.showcaseStorageId);
            if (showcaseStorage == null) return ShowcaseStorageDoesNotExist(d.showcaseStorageId);

            Item item = await ctx.Items.FindAsync(d.itemId);
            if (item == null) return Errors.DoesNotExist(d.itemId, localizationSettings.Crafting.Item);

            if (showcaseStorage.Items == null || !showcaseStorage.Items.Any(priceItem => priceItem.StorageItem.Item == item))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The storage does not have this item",
                    RussianMessage = "На складе нет такого предмета"
                };
            }

            StorageItemWithPrice itemInStorage = showcaseStorage.Items.Single(priceItem => priceItem.StorageItem.Item == item);

            if (itemInStorage.StorageItem.Quantity <= d.quantity)
            {
                ctx.StorageItemsWithPrice.Remove(itemInStorage);
            }
            else
            {
                itemInStorage.StorageItem.Quantity -= d.quantity;
            }

            await ctx.SaveChangesAsync();

            return (await ctx.ShowcaseStorages.FindAsync(d.showcaseStorageId)).Items;
        }


        [NonAction]
        private LocalizedError ShowcaseStorageDoesNotExist(long id)
        {
            return Errors.DoesNotExist(id, modelLocalization);
        }
    }
}
