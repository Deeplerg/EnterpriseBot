using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Game.Essences;
using EnterpriseBot.Api.Models.ModelCreationParams.Storages;
using EnterpriseBot.Api.Models.Other;
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
    public class InventoryStorage
    {
        protected InventoryStorage() { }

        #region model
        public long Id { get; protected set; }

        public virtual Player OwningPlayer { get; protected set; }

        public decimal Capacity { get => Storage.Capacity; }
        public decimal AvailableSpace { get => Storage.AvailableSpace; }
        public decimal OccupiedSpace { get => Storage.OccupiedSpace; }
        public IReadOnlyCollection<StorageItem> Items { get => Storage.Items; }

        protected virtual Storage Storage { get; set; }

        #region errors
        #endregion
        #endregion

        #region actions
        public static GameResult<InventoryStorage> Create(InventoryStorageCreationParams pars)
        {
            var storageCreationResult = Storage.Create(new StorageCreationParams
            {
                Capacity = pars.Capacity,
            });
            if (storageCreationResult.LocalizedError != null) return storageCreationResult.LocalizedError;

            Storage storage = storageCreationResult;

            return new InventoryStorage
            {
                OwningPlayer = pars.OwningPlayer,
                Storage = storage
            };
        }

        public GameResult<int> Add(Item item, int quantity = 1)
        {
            return Storage.Add(item, quantity);
        }

        public GameResult<int> Add(StorageItem storageItem)
        {
            return Storage.Add(storageItem);
        }

        public GameResult<int> Add(IEnumerable<StorageItem> storageItems)
        {
            return Storage.Add(storageItems);
        }


        public GameResult<int> Remove(Item item, int quantity = 1)
        {
            return Storage.Remove(item, quantity);
        }

        public GameResult<int> Remove(StorageItem storageItem)
        {
            return Storage.Remove(storageItem);
        }


        public GameResult<int> TransferTo(Storage storage, Item item, int quantity = 1)
        {
            return Storage.TransferTo(storage, item, quantity);
        }

        public GameResult<int> TransferTo(Storage storage, StorageItem storageItem)
        {
            return Storage.TransferTo(storage, storageItem);
        }

        public GameResult<int> TransferTo(Storage storage, IEnumerable<StorageItem> items)
        {
            return Storage.TransferTo(storage, items);
        }

        public GameResult<int> TransferEverythingTo(Storage storage)
        {
            return Storage.TransferEverythingTo(storage);
        }

        public GameResult<int> TransferEverythingTo(Storage storage, Item itemTypeToTransfer)
        {
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

        public GameResult<decimal> UpgradeCapacity(GameplaySettings settings, DonationSettings donationSettings)
        {
            var upgradePrice = settings.Storages.Inventory.UpgradePrice;
            decimal step = settings.Storages.Inventory.UpgradeStep;

            if (Capacity >= settings.Storages.Company.MaxCapacity)
            {
                return Errors.StorageCapacityIsMax;
            }

            var reduceResult = OwningPlayer.Purse.Reduce(upgradePrice.Amount, upgradePrice.Currency);
            if (reduceResult.LocalizedError != null) return reduceResult.LocalizedError;

            var addCapacityResult = Storage.AddCapacity(step);
            if (addCapacityResult.LocalizedError != null) return addCapacityResult.LocalizedError;

            return Capacity;
        }

        public static implicit operator Storage(InventoryStorage inventoryStorage)
        {
            return inventoryStorage.Storage;
        }
        #endregion
    }
}
