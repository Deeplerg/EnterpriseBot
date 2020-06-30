﻿using EnterpriseBot.Api.Game.Business.Company;
using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Game.Essences;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.ModelCreationParams.Storages;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings.BusinessPricesSettings;
using EnterpriseBot.Api.Models.Settings.DonationSettings;
using EnterpriseBot.Api.Models.Settings.GameplaySettings;
using EnterpriseBot.Api.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Game.Storages
{
    public class ShowcaseStorage
    {
        protected ShowcaseStorage() { }

        #region model
        public long Id { get; protected set; }

        public virtual Company OwningCompany { get; protected set; }

        public virtual IReadOnlyCollection<ItemPrice> Prices
        {
            get => new ReadOnlyCollection<ItemPrice>(prices);
            protected set => prices = value.ToList();
        }
        private List<ItemPrice> prices = new List<ItemPrice>();

        public decimal Capacity { get => Storage.Capacity; }
        public decimal AvailableSpace { get => Storage.AvailableSpace; }
        public decimal OccupiedSpace { get => Storage.OccupiedSpace; }
        public IReadOnlyCollection<StorageItem> Items { get => Storage.Items; }

        protected virtual Storage Storage { get; set; }

        #region errors
        private static readonly LocalizedError priceNotDefinedError = new LocalizedError
        {
            ErrorSeverity = ErrorSeverity.Normal,
            EnglishMessage = "The price for this item must be added first",
            RussianMessage = "Сначала необходимо выставить цену на этот предмет"
        };
        #endregion
        #endregion

        #region actions
        public static GameResult<ShowcaseStorage> Create(ShowcaseStorageCreationParams pars)
        {
            var storageCreationResult = Storage.Create(new StorageCreationParams
            {
                Capacity = pars.Capacity
            });
            if (storageCreationResult.LocalizedError != null) return storageCreationResult.LocalizedError;

            Storage storage = storageCreationResult;

            return new ShowcaseStorage
            {
                OwningCompany = pars.OwningCompany,
                Storage = storage
            };
        }

        public static EmptyGameResult Buy(Company company, 
            BusinessPricesSettings pricesSettings, DonationSettings donationSettings, Player invoker = null)
        {
            if(!invoker.HasPermission(CompanyJobPermissions.BuyStorages, company))
            {
                return Errors.DoesNotHavePermission();
            }

            decimal price = pricesSettings.CompanyFeatures.NewStoragePrices.Showcase;

            var reduceResult = company.ReduceBusinessCoins(price, donationSettings, invoker);
            if (reduceResult.LocalizedError != null) return reduceResult.LocalizedError;

            return new EmptyGameResult();
        }

        public GameResult<int> Add(StorageItem storageItem, Player invoker = null)
        {
            if (!HasPermissionToManage(invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            if (!IsPriceDefinedForItem(storageItem.Item))
            {
                return priceNotDefinedError;
            }

            return Storage.Add(storageItem);
        }

        public GameResult<int> Add(Item item, int quantity = 1, Player invoker = null)
        {
            if (!HasPermissionToManage(invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            if (!IsPriceDefinedForItem(item))
            {
                return priceNotDefinedError;
            }

            return Storage.Add(item, quantity);
        }


        public GameResult<int> Remove(StorageItem storageItem, Player invoker = null)
        {
            if (!HasPermissionToManage(invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            return Storage.Remove(storageItem);
        }

        public GameResult<int> Remove(Item item, int quantity = 1, Player invoker = null)
        {
            if (!HasPermissionToManage(invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            return Storage.Remove(item, quantity);
        }


        public GameResult<int> TransferTo(Storage storage, Item item, int quantity = 1, Player invoker = null)
        {
            if (!HasPermissionToManage(invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            return Storage.TransferTo(storage, item, quantity);
        }

        public GameResult<int> TransferTo(Storage storage, StorageItem storageItem, Player invoker = null)
        {
            if (!HasPermissionToManage(invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            return Storage.TransferTo(storage, storageItem);
        }

        public GameResult<int> TransferTo(Storage storage, IEnumerable<StorageItem> items, Player invoker = null)
        {
            if (!HasPermissionToManage(invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            return Storage.TransferTo(storage, items);
        }

        public GameResult<int> TransferEverythingTo(Storage storage, Player invoker = null)
        {
            if (!HasPermissionToManage(invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            return Storage.TransferEverythingTo(storage);
        }

        public GameResult<int> TransferEverythingTo(Storage storage, Item itemTypeToTransfer, Player invoker = null)
        {
            if (!HasPermissionToManage(invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            return Storage.TransferEverythingTo(storage, itemTypeToTransfer);
        }


        public GameResult<ItemPrice> AddPrice(Item item, decimal price)
        {
            var itemPriceCreationResult = ItemPrice.Create(new ItemPriceCreationParams
            {
                Item = item,
                Price = price
            });
            if (itemPriceCreationResult.LocalizedError != null) return itemPriceCreationResult.LocalizedError;

            ItemPrice itemPrice = itemPriceCreationResult;

            if (Prices == null) 
                Prices = new List<ItemPrice>();

            prices.Add(itemPrice);

            return itemPrice;
        }

        public GameResult<ItemPrice> SetPrice(Item item, decimal newPrice)
        {
            if (Prices != null)
            {
                var existingPrice = Prices.SingleOrDefault(price => price.Item == item);
                if (existingPrice != null)
                {
                    var priceChangeResult = existingPrice.SetPrice(newPrice);
                    if (priceChangeResult.LocalizedError != null) return priceChangeResult.LocalizedError;

                    return existingPrice;
                }
            }

            return AddPrice(item, newPrice);
        }

        public GameResult<ItemPrice> GetPrice(Item item)
        {
            var price = Prices.SingleOrDefault(price => price.Item == item);
            if(price == null)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Price is not defined for this item",
                    RussianMessage = "Цена на этот предмет не установлена"
                };
            }

            return price;
        }

        public bool IsPriceDefinedForItem(Item item)
        {
            return Prices != null && Prices.Any(price => price.Item == item);
        }

        public GameResult<bool> Contains(StorageItem storageItem)
        {
            return Storage.Contains(storageItem);
        }

        public GameResult<StorageItem> GetItem(Item item)
        {
            return Storage.GetItem(item);
        }

        public GameResult<int> BuyItem(Item item, int quantity, Player buyer)
        {
            if(quantity < 1)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Item quantity can't be lower than 1",
                    RussianMessage = "Количество предметов не может быть ниже 1"
                };
            }

            var getPriceResult = GetPrice(item);
            if (getPriceResult.LocalizedError != null) return getPriceResult.LocalizedError;

            ItemPrice itemPrice = getPriceResult;
            decimal price = itemPrice.Price * quantity;

            var reduceResult = buyer.Purse.Reduce(price, Currency.Units);
            if (reduceResult.LocalizedError != null) return reduceResult.LocalizedError;

            return TransferTo(buyer.Inventory, item, quantity);
        }

        public GameResult<decimal> UpgradeCapacity(GameplaySettings settings, DonationSettings donationSettings, Player invoker = null)
        {
            if (!HasPermission(CompanyJobPermissions.UpgradeStorages, invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            var upgradePrice = settings.Storages.Trunk.UpgradePrice;
            decimal step = settings.Storages.Trunk.UpgradeStep;

            if (Capacity >= settings.Storages.Company.MaxCapacity)
            {
                return Errors.StorageCapacityIsMax;
            }

            if (upgradePrice.Currency == Currency.BusinessCoins)
            {
                var reduceResult = OwningCompany.ReduceBusinessCoins(upgradePrice.Amount, donationSettings);
                if (reduceResult.LocalizedError != null) return reduceResult.LocalizedError;
            }
            else
            {
                var reduceResult = OwningCompany.Purse.Reduce(upgradePrice.Amount, upgradePrice.Currency);
                if (reduceResult.LocalizedError != null) return reduceResult.LocalizedError;
            }

            var addCapacityResult = Storage.AddCapacity(step);
            if (addCapacityResult.LocalizedError != null) return addCapacityResult.LocalizedError;

            return Capacity;
        }

        public static implicit operator Storage(ShowcaseStorage showcaseStorage)
        {
            return showcaseStorage.Storage;
        }


        private bool HasPermissionToManage(Player invoker)
        {
            return HasPermission(CompanyJobPermissions.ManageTrunkStorages, invoker);
        }

        private bool HasPermission(CompanyJobPermissions permission, Player invoker)
        {
            if (invoker == null)
                return true;

            return invoker.HasPermission(permission, OwningCompany);
        }
        #endregion
    }
}
