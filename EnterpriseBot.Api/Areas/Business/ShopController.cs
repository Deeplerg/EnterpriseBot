using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Models.Common.Business;
using EnterpriseBot.Api.Models.Common.Essences;
using EnterpriseBot.Api.Models.Common.Storages;
using EnterpriseBot.Api.Models.Contexts;
using EnterpriseBot.Api.Models.ModelCreationParams.Business;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings.GameplaySettings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using static EnterpriseBot.Api.Utils.Constants;
using static EnterpriseBot.Api.Utils.Miscellaneous;

namespace EnterpriseBot.Api.Areas.Business
{
    [Area(nameof(Business))]
    public class ShopController : Controller, IGameController<Shop>
    {
        private readonly ApplicationContext ctx;
        private readonly GameplaySettings gameplaySettings;
        private readonly LocalizationSettings localizationSettings;

        private readonly LocalizationSetting modelLocalization;

        public ShopController(ApplicationContext context,
                             IOptions<GameplaySettings> gameplayOptions,
                             IOptions<LocalizationSettings> localizationOptions)
        {
            this.ctx = context;
            this.gameplaySettings = gameplayOptions.Value;
            this.localizationSettings = localizationOptions.Value;

            modelLocalization = localizationSettings.Business.Shop;
        }

        /// <inheritdoc/>
        [HttpPost]
        public async Task<GameResult<Shop>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            Shop shop = await ctx.Shops.FindAsync(id);
            //if (shop == null) return ShopDoesNotExist(id);

            return shop;
        }

        ///// <inheritdoc/>
        [HttpPost]
        public async Task<GameResult<Shop>> Create([FromBody] ShopCreationParams cp)
        {
            return await CreateShop(cp);
        }

        /// <summary>
        /// Withdraws units from the future general manager and creates a new shop
        /// </summary>
        /// <returns>Created shop</returns>
        /// <remarks>Customer is a general manager, so there's no need to specify one separately</remarks>
        [HttpPost]
        public async Task<GameResult<Shop>> Buy([FromBody] ShopCreationParams cp)
        {
            Player generalManager = await ctx.Players.FindAsync(cp.GeneralManagerId);
            if (generalManager == null) return Errors.DoesNotExist(cp.GeneralManagerId, localizationSettings.Essences.Player);

            decimal shopPrice = gameplaySettings.Prices.Shop.DefaultPrice;
            if (generalManager.Units < shopPrice)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Not enough units to create a shop",
                    RussianMessage = "Недостаточно юнитов для создания магазина"
                };
            }

            generalManager.Units -= shopPrice;

            return await CreateShop(cp);
        }

        /// <summary>
        /// Changes shop owner
        /// </summary>
        [HttpPost]
        public async Task<EmptyGameResult> ChangeOwner([FromBody] string json)
        {
            var pars = new
            {
                shopId = default(long),
                newOwnerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Shop shop = await ctx.Shops.FindAsync(d.shopId);
            if (shop == null) return ShopDoesNotExist(d.shopId);

            Player newOwner = await ctx.Players.FindAsync(d.newOwnerId);
            if (newOwner == null) return Errors.DoesNotExist(d.newOwnerId, localizationSettings.Essences.Player);

            if (shop.GeneralManagerId == newOwner.Id)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Can’t transfer shop ownership to yourself",
                    RussianMessage = "Нельзя передать магазин себе самому"
                };
            }

            shop.GeneralManager = newOwner;

            await ctx.SaveChangesAsync();

            return new EmptyGameResult();
        }

        /// <summary>
        /// Changes shop description
        /// </summary>
        /// <returns>New description</returns>
        [HttpPost]
        public async Task<GameResult<string>> ChangeDescription([FromBody] string json)
        {
            var pars = new
            {
                shopId = default(long),
                newDescription = default(string)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Shop shop = await ctx.Shops.FindAsync(d.shopId);
            if (shop == null) return ShopDoesNotExist(d.shopId);

            if (!CheckDescription(d.newDescription))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Shop description has not passed verification. Please check the input and try again",
                    RussianMessage = "Описание магазина не прошло проверку. Пожалуйста, проверьте ввод и попробуйте ещё раз"
                };
            }

            shop.Description = d.newDescription;

            await ctx.SaveChangesAsync();

            return shop.Description;
        }

        /// <summary>
        /// Withdraws all shop units to the owner's account
        /// </summary>
        /// <returns>Shop units</returns>
        [HttpPost]
        public async Task<GameResult<decimal>> WithdrawUnits([FromBody] string json)
        {
            var pars = new
            {
                shopId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Shop shop = await ctx.Shops.FindAsync(d.shopId);
            if (shop == null) return ShopDoesNotExist(d.shopId);

            Player generalManager = shop.GeneralManager;
            if (generalManager == null)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Critical,
                    EnglishMessage = "Shop must have general manager",
                    RussianMessage = "У магазина должен быть генеральный директор"
                };
            }

            if (shop.ShopUnits <= 0)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Unable to withdraw a null or negative amount of units",
                    RussianMessage = "Невозможно снять нулевое или отрицательное количество юнитов"
                };
            }

            decimal unitsToWithdraw = shop.ShopUnits;
            shop.ShopUnits -= unitsToWithdraw;
            generalManager.Units += unitsToWithdraw;

            await ctx.SaveChangesAsync();

            return (await ctx.Shops.FindAsync(d.shopId)).ShopUnits;
        }

        /// <summary>
        /// Adds specified amount of units to the shop account
        /// </summary>
        /// <returns>Shop's units after adding</returns>
        [HttpPost]
        public async Task<GameResult<decimal>> AddUnits([FromBody] string json)
        {
            var pars = new
            {
                shopId = default(long),
                amount = default(decimal)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Shop shop = await ctx.Shops.FindAsync(d.shopId);
            if (shop == null) return ShopDoesNotExist(d.shopId);

            if (d.amount <= 0)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Unable to add a null or negative amount of units",
                    RussianMessage = "Невозможно добавить нулевое или отрицательное количество юнитов"
                };
            }

            shop.ShopUnits += d.amount;

            await ctx.SaveChangesAsync();

            return (await ctx.Shops.FindAsync(d.shopId)).ShopUnits;
        }

        /// <summary>
        /// Adds an item to player inventory and takes units for the purchase, also adding them to the shop account
        /// </summary>
        /// <returns>Player's units after making the purchase</returns>
        [HttpPost]
        public async Task<GameResult<decimal>> BuyItem([FromBody] string json)
        {
            var pars = new
            {
                shopId = default(long),
                playerId = default(long),
                itemId = default(long),
                amount = default(int)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            if (d.amount <= 0)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Unable to buy a null or negative amount of items",
                    RussianMessage = "Нельзя купить нулевое или отрицательное количество предметов"
                };
            }

            Shop shop = await ctx.Shops.FindAsync(d.shopId);
            if (shop == null) return ShopDoesNotExist(d.shopId);

            Player player = await ctx.Players.FindAsync(d.playerId);
            if (player == null) return Errors.DoesNotExist(d.playerId, localizationSettings.Essences.Player);

            StorageItemWithPrice itemWithPrice = await ctx.StorageItemsWithPrice.FindAsync(d.itemId);
            if (itemWithPrice == null) return Errors.DoesNotExist(d.itemId, localizationSettings.Storages.StorageItemWithPrice);

            if (shop.ShowcaseStorage.Items == null || !shop.ShowcaseStorage.Items.Contains(itemWithPrice))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "This shop does not have this item",
                    RussianMessage = "В этом магазине нет такого предмета"
                };
            }

            decimal totalPrice = itemWithPrice.Price * d.amount;
            if (player.Units < totalPrice)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Not enough units to buy this item",
                    RussianMessage = "Недостаточно юнитов для покупки"
                };
            }

            if (d.amount > itemWithPrice.StorageItem.Quantity)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The shop does not have enough items",
                    RussianMessage = "В магазине нет столько предметов"
                };
            }

            int itemSpace = itemWithPrice.StorageItem.Item.Space;
            int occupiedSpace = player.PersonalStorage.Items.Sum(item => item.Quantity);

            if (occupiedSpace + (itemSpace * d.amount) > player.PersonalStorage.Capacity)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Not enough space in the inventory",
                    RussianMessage = "Недостаточно места в инвентаре"
                };
            }


            player.Units -= totalPrice;
            shop.ShopUnits += totalPrice;

            if (itemWithPrice.StorageItem.Quantity - d.amount == 0)
            {
                ctx.StorageItemsWithPrice.Remove(itemWithPrice);
            }
            else
            {
                itemWithPrice.StorageItem.Quantity -= d.amount;
            }

            player.PersonalStorage.Items.Add(new StorageItem
            {
                Item = itemWithPrice.StorageItem.Item,
                Quantity = d.amount
            });

            await ctx.SaveChangesAsync();

            return (await ctx.Players.FindAsync(d.playerId)).Units;
        }

        /// <summary>
        /// Returns a shop which name matches the specified one
        /// </summary>
        /// <returns>Shop which name matches the specified one</returns>
        [HttpPost]
        public async Task<GameResult<Shop>> GetByName([FromBody] string json)
        {
            var pars = new
            {
                name = default(string)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Shop shop = await ctx.Shops.FirstOrDefaultAsync(s => s.Name.ToLower().Trim() == d.name.ToLower().Trim());
            //if (shop == null) return ShopDoesNotExist(d.name);

            return shop;
        }


        [NonAction]
        private LocalizedError ShopDoesNotExist(long id)
        {
            return Errors.DoesNotExist(id, modelLocalization);
        }

        [NonAction]
        private async Task<GameResult<Shop>> CreateShop(ShopCreationParams cp)
        {
            UserInputRequirements req = localizationSettings.UserInputRequirements;

            if (!CheckBusinessName(cp.Name))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Shop name has not passed verification. " + string.Format(req.Name.English, BusinessNameMaxLength),
                    RussianMessage = "Название магазина не прошло проверку. " + string.Format(req.Name.Russian, BusinessNameMaxLength)
                };
            }
            if (!CheckDescription(cp.Description))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Shop description has not passed verification. " + string.Format(req.Description.English, DescriptionMaxLength),
                    RussianMessage = "Описание магазина не прошло проверку. " + string.Format(req.Description.Russian, DescriptionMaxLength)
                };
            }

            //can be optimized by removing "ToList", which is not recommended (won't generate request with Trim and string.Equals)
            if ((await ctx.Shops.ToListAsync()).Any(shop => CompareNames(shop.Name, cp.Name))
            || (await ctx.Companies.ToListAsync()).Any(company => CompareNames(company.Name, cp.Name)))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "A shop or a company with the same name already exists",
                    RussianMessage = "Магазин или компания с таким названием уже существует"
                };
            }

            Player generalManager = await ctx.Players.FindAsync(cp.GeneralManagerId);
            if (generalManager == null) return Errors.DoesNotExist(cp.GeneralManagerId, localizationSettings.Essences.Player);

            IncomeStorage createdIncomeStorage = (await ctx.IncomeStorages.AddAsync(
                new IncomeStorage
                {
                    Capacity = gameplaySettings.Storages.Income.DefaultCapacity
                })).Entity;

            ShowcaseStorage createdShowcaseStorage = (await ctx.ShowcaseStorages.AddAsync(
                new ShowcaseStorage
                {
                    Capacity = gameplaySettings.Storages.Showcase.DefaultCapacity
                })).Entity;

            Shop createdShop = (await ctx.Shops.AddAsync(
                new Shop
                {
                    Name = cp.Name,
                    Description = cp.Description,
                    GeneralManager = generalManager,
                    ShopUnits = gameplaySettings.Shop.DefaultUnits,
                    IncomeStorage = createdIncomeStorage,
                    ShowcaseStorage = createdShowcaseStorage
                })).Entity;

            await ctx.SaveChangesAsync();

            return await ctx.Shops.FindAsync(createdShop.Id);
        }
    }
}
