using EnterpriseBot.Api.Game.Business.Company;
using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Game.Essences;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.ModelCreationParams.Storages;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings;
using EnterpriseBot.Api.Models.Settings.BusinessPricesSettings;
using EnterpriseBot.Api.Models.Settings.DonationSettings;
using EnterpriseBot.Api.Models.Settings.GameplaySettings;
using EnterpriseBot.Api.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Game.Storages
{
    public class CompanyStorage
    {
        protected CompanyStorage() { }

        #region model
        public long Id { get; protected set; }

        public virtual Company OwningCompany { get; protected set; }
        
        public CompanyStorageType Type { get; protected set; }

        public decimal Capacity { get => Storage.Capacity; }
        public decimal AvailableSpace { get => Storage.AvailableSpace; }
        public decimal OccupiedSpace { get => Storage.OccupiedSpace; }
        public IReadOnlyCollection<StorageItem> Items { get => Storage.Items; }

        protected virtual Storage Storage { get; set; }

        #region errors
        #endregion
        #endregion

        #region actions
        public static GameResult<CompanyStorage> Create(CompanyStorageCreationParams pars)
        {
            var storageCreationResult = Storage.Create(new StorageCreationParams
            {
                Capacity = pars.Capacity,
            });
            if (storageCreationResult.LocalizedError != null) return storageCreationResult.LocalizedError;

            Storage storage = storageCreationResult;

            return new CompanyStorage
            {
                OwningCompany = pars.OwningCompany,
                Storage = storage,
                Type = pars.Type
            };
        }

        public static EmptyGameResult Buy(Company company, CompanyStorageType storageType, GameSettings gameSettings, Player invoker = null)
        {
            var prices = gameSettings.BusinessPrices.CompanyFeatures;

            if (!invoker.HasPermission(CompanyJobPermissions.BuyStorages, company))
            {
                return Errors.DoesNotHavePermission();
            }

            decimal price;
            switch(storageType)
            {
                case CompanyStorageType.General:
                    price = prices.NewStoragePrices.Company;
                    break;

                case CompanyStorageType.Income:
                    price = prices.NewStoragePrices.Income;
                    break;

                default:
                    return Errors.UnknownEnumValue(storageType);
            }

            var reduceResult = company.ReduceBusinessCoins(price, gameSettings, invoker);
            if (reduceResult.LocalizedError != null) return reduceResult.LocalizedError;

            return new EmptyGameResult();
        }

        public GameResult<int> Add(Item item, int quantity = 1, Player invoker = null)
        {
            if(!HasPermissionToManage(invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            return Storage.Add(item, quantity);
        }

        public GameResult<int> Add(StorageItem storageItem, Player invoker = null)
        {
            if (!HasPermissionToManage(invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            return Storage.Add(storageItem);
        }

        public GameResult<int> Add(IEnumerable<StorageItem> storageItems, Player invoker = null)
        {
            if (!HasPermissionToManage(invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            return Storage.Add(storageItems);
        }


        public GameResult<int> Remove(Item item, int quantity = 1, Player invoker = null)
        {
            if (!HasPermissionToManage(invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            return Storage.Remove(item, quantity);
        }

        public GameResult<int> Remove(StorageItem storageItem, Player invoker = null)
        {
            if (!HasPermissionToManage(invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            return Storage.Remove(storageItem);
        }

        public GameResult<int> Remove(IEnumerable<Ingredient> ingredients, Player invoker = null)
        {
            if (!HasPermissionToManage(invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            return Storage.Remove(ingredients);
        }


        public GameResult<int> TransferHereFrom(Storage storage, Item item, int quantity = 1, Player invoker = null)
        {
            if (!HasPermissionToManage(invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            return storage.TransferTo(Storage, item, quantity);
        }

        public GameResult<int> TransferHereFrom(Storage storage, StorageItem storageItem, Player invoker = null)
        {
            if (!HasPermissionToManage(invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            return storage.TransferTo(Storage, storageItem);
        }

        public GameResult<int> TransferHereFrom(Storage storage, IEnumerable<StorageItem> items, Player invoker = null)
        {
            if (!HasPermissionToManage(invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            return storage.TransferTo(Storage, items);
        }

        public GameResult<int> TransferEverythingHereFrom(Storage storage, Player invoker = null)
        {
            if (!HasPermissionToManage(invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            return storage.TransferEverythingTo(Storage);
        }

        public GameResult<int> TransferEverythingHereFrom(Storage storage, Item itemTypeToTransfer, Player invoker = null)
        {
            if (!HasPermissionToManage(invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            return storage.TransferEverythingTo(Storage, itemTypeToTransfer);
        }


        public GameResult<bool> Contains(Item item, int quantity = 1)
        {
            return Storage.Contains(item, quantity);
        }

        public GameResult<bool> Contains(StorageItem storageItem)
        {
            return Storage.Contains(storageItem);
        }

        public GameResult<StorageItem> GetItem(Item item)
        {
            return Storage.GetItem(item);
        }

        public GameResult<bool> HasIngredients(IEnumerable<Ingredient> ingredients)
        {
            return ingredients.All(ing => Items.Any(storageItem => storageItem.Item == ing.Item && storageItem.Quantity >= ing.Quantity));
        }

        public GameResult<decimal> UpgradeCapacity(GameplaySettings settings, GameSettings gameSettings, Player invoker = null)
        {
            if (!HasPermission(CompanyJobPermissions.UpgradeStorages, invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            var upgradePrice = settings.Storage.Company.UpgradePrice;
            decimal step = settings.Storage.Company.UpgradeStep;

            if (Capacity >= settings.Storage.Company.MaxCapacity)
            {
                return Errors.StorageCapacityIsMax;
            }

            if (upgradePrice.Currency == Currency.BusinessCoins)
            {
                var reduceResult = OwningCompany.ReduceBusinessCoins(upgradePrice.Amount, gameSettings);
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


        private bool HasPermissionToManage(Player invoker)
        {
            return HasPermission(CompanyJobPermissions.ManageCompanyStorages, invoker);
        }

        private bool HasPermission(CompanyJobPermissions permission, Player invoker)
        {
            if (invoker == null)
                return true;

            return invoker.HasPermission(permission, OwningCompany);
        }

        public static implicit operator Storage(CompanyStorage companyStorage)
        {
            return companyStorage.Storage;
        }
        #endregion
    }
}
