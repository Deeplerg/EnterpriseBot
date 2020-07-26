using EnterpriseBot.Api.Game.Business.Company;
using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Game.Essences;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.ModelCreationParams.Storages;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings;
using EnterpriseBot.Api.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace EnterpriseBot.Api.Game.Storages
{
    // Trying to "simulate" inheritance while TPT is not yet supported in EF Core
    public class TrunkStorage
    {
        protected TrunkStorage() { }

        #region model
        public long Id { get; protected set; }

        public virtual Truck OwningTruck { get; protected set; }

        public decimal Capacity { get => Storage.Capacity; }
        public decimal AvailableSpace { get => Storage.AvailableSpace; }
        public decimal OccupiedSpace { get => Storage.OccupiedSpace; }
        public IReadOnlyCollection<StorageItem> Items { get => Storage.Items; }

        [JsonIgnore]
        protected virtual Storage Storage { get; set; }
        public long StorageId { get; protected set; }

        #region errors
        #endregion
        #endregion

        #region actions
        public static GameResult<TrunkStorage> Create(TrunkStorageCreationParams pars)
        {
            var storageCreationResult = Storage.Create(new StorageCreationParams
            {
                Capacity = pars.Capacity,
            });
            if (storageCreationResult.LocalizedError != null) return storageCreationResult.LocalizedError;

            Storage storage = storageCreationResult;

            return new TrunkStorage
            {
                OwningTruck = pars.OwningTruck,
                Storage = storage
            };
        }

        public GameResult<int> Add(Item item, int quantity = 1, Player invoker = null)
        {
            if (!HasPermissionToManage(invoker))
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

        public GameResult<decimal> UpgradeCapacity(GameSettings gameSettings, Player invoker = null)
        {
            var storageGameplaySettings = gameSettings.Gameplay.Storage;

            if (!HasPermission(CompanyJobPermissions.UpgradeStorages, invoker))
            {
                return Errors.DoesNotHavePermission();
            }

            var upgradePrice = storageGameplaySettings.Trunk.UpgradePrice;
            decimal step = storageGameplaySettings.Trunk.UpgradeStep;

            if (Capacity >= storageGameplaySettings.Company.MaxCapacity)
            {
                return Errors.StorageCapacityIsMax;
            }

            if (upgradePrice.Currency == Currency.BusinessCoins)
            {
                var reduceResult = OwningTruck.TruckGarage.Company.ReduceBusinessCoins(upgradePrice.Amount, gameSettings);
                if (reduceResult.LocalizedError != null) return reduceResult.LocalizedError;
            }
            else
            {
                var reduceResult = OwningTruck.TruckGarage.Company.Purse.Reduce(upgradePrice.Amount, upgradePrice.Currency);
                if (reduceResult.LocalizedError != null) return reduceResult.LocalizedError;
            }

            var addCapacityResult = Storage.AddCapacity(step);
            if (addCapacityResult.LocalizedError != null) return addCapacityResult.LocalizedError;

            return Capacity;
        }

        public EmptyGameResult ReturnErrorIfDoesNotHavePermissionToManage(Player invoker)
        {
            if (!HasPermissionToManage(invoker))
            {
                return Errors.DoesNotHavePermission();
            }
            else
            {
                return new EmptyGameResult();
            }
        }

        public bool HasPermissionToManage(Player invoker)
        {
            return HasPermission(CompanyJobPermissions.ManageTrunkStorages, invoker);
        }


        private bool HasPermission(CompanyJobPermissions permission, Player invoker)
        {
            return invoker.HasPermission(permission, OwningTruck.TruckGarage.Company);
        }

        public static implicit operator Storage(TrunkStorage trunkStorage)
        {
            return trunkStorage.Storage;
        }
        #endregion
    }
}
