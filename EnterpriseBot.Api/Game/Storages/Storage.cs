using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Models.ModelCreationParams.Storages;
using EnterpriseBot.Api.Models.Other;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EnterpriseBot.Api.Game.Storages
{
    public class Storage
    {
        protected Storage() { }

        #region model
        public long Id { get; protected set; }

        public decimal Capacity { get; protected set; }

        public virtual IReadOnlyCollection<StorageItem> Items
        {
            get => new ReadOnlyCollection<StorageItem>(items);
            protected set => items = value.ToList();
        }
        private List<StorageItem> items = new List<StorageItem>();

        public decimal AvailableSpace
        {
            get => Capacity - OccupiedSpace;
        }
        public decimal OccupiedSpace
        {
            get => Items.Sum(item => item.Space);
        }
        #endregion

        #region errors
        private static readonly LocalizedError itemQuantityLowerThan1Error = new LocalizedError
        {
            ErrorSeverity = ErrorSeverity.Normal,
            EnglishMessage = "Item quanitity can't be lower than or equal to 0",
            RussianMessage = "Количество предметов не может быть меньше чем или равно 0"
        };
        #endregion
        #region actions
        public static GameResult<Storage> Create(StorageCreationParams pars)
        {
            if (pars.Capacity <= 0)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal, // Critical ??
                    EnglishMessage = "Storage capacity can't be lower than or equal to 0",
                    RussianMessage = "Вместимость хранилища не может быть меньше или равна 0"
                };
            }

            return new Storage
            {
                Capacity = pars.Capacity
            };
        }

        public GameResult<int> Add(Item item, int quantity = 1)
        {
            var storageItemCreationResult = StorageItem.Create(new StorageItemCreationParams
            {
                Item = item,
                Quantity = quantity
            });
            if (storageItemCreationResult.LocalizedError != null) return storageItemCreationResult.LocalizedError;

            StorageItem storageItem = storageItemCreationResult;

            return Add(storageItem);
        }

        public GameResult<int> Add(StorageItem storageItem)
        {
            if (storageItem.Quantity < 1)
            {
                return itemQuantityLowerThan1Error;
            }
            //if(AvailableSpace < storageItem.Space)
            //{
            //    return new LocalizedError
            //    {
            //        ErrorSeverity = ErrorSeverity.Normal,
            //        EnglishMessage = "Not enough space in a storage",
            //        RussianMessage = "В хранилище недостаточно места"
            //    };
            //}

            var existingStorageItem = items.FirstOrDefault(item => item.Item == storageItem.Item);

            if (existingStorageItem is null)
            {
                if (AvailableSpace >= storageItem.Space)
                {
                    items.Add(storageItem);
                    return storageItem.Quantity;
                }
                else
                {
                    if (AvailableSpace >= storageItem.Item.Space)
                    {
                        var storageItemCreationResult = StorageItem.Create(new StorageItemCreationParams
                        {
                            Item = storageItem.Item,
                            Quantity = (int)Math.Floor(AvailableSpace / storageItem.Item.Space)
                        });
                        if (storageItemCreationResult.LocalizedError != null) return storageItemCreationResult.LocalizedError;

                        StorageItem createdStorageItem = storageItemCreationResult;

                        items.Add(createdStorageItem);

                        return createdStorageItem.Quantity;
                    }
                    else
                    {
                        //return new LocalizedError
                        //{
                        //    ErrorSeverity = ErrorSeverity.Normal,
                        //    EnglishMessage = "Not enough space in a storage",
                        //    RussianMessage = "В хранилище недостаточно места"
                        //};
                        return 0;
                    }
                }
            }
            else
            {
                int quantity;

                if (AvailableSpace >= existingStorageItem.Space)
                    quantity = (int)Math.Floor(existingStorageItem.Space);
                else
                {
                    if (AvailableSpace >= storageItem.Item.Space)
                        quantity = (int)Math.Floor(AvailableSpace / storageItem.Item.Space);
                    else
                        return 0;
                }

                var addResult = existingStorageItem.AddQuantity(quantity);
                if (addResult.LocalizedError != null) return addResult.LocalizedError;

                return quantity;
            }
        }

        public GameResult<int> Add(IEnumerable<StorageItem> storageItems)
        {
            int addedQuantity = 0;

            foreach (var item in storageItems)
            {
                var addResult = Add(item);
                if (addResult.LocalizedError != null) return addResult.LocalizedError;

                addedQuantity += addResult;
            }

            return addedQuantity;
        }


        public GameResult<int> Remove(Item item, int quantity = 1)
        {
            var storageItemCreationResult = StorageItem.Create(new StorageItemCreationParams
            {
                Item = item,
                Quantity = quantity
            });
            if (storageItemCreationResult.LocalizedError != null) return storageItemCreationResult.LocalizedError;

            StorageItem storageItem = storageItemCreationResult;

            return Remove(storageItem);
        }

        public GameResult<int> Remove(StorageItem storageItem)
        {
            if (storageItem.Quantity < 1)
            {
                return itemQuantityLowerThan1Error;
            }

            //if Items have the exact same item, it is possible to simply remove it
            var existingStorageItem = items.FirstOrDefault(item => item == storageItem);
            if (existingStorageItem != null)
            {
                int removeQuantity = existingStorageItem.Quantity;
                items.Remove(existingStorageItem);

                return removeQuantity;
            }

            //if Items have the item with the same type
            existingStorageItem = items.FirstOrDefault(item => item.Item == storageItem.Item);
            if (existingStorageItem is null)
            {
                return 0; //nothing removed
            }
            else
            {
                int quantity;

                if (existingStorageItem.Quantity < storageItem.Quantity)
                    quantity = existingStorageItem.Quantity;
                else
                    quantity = storageItem.Quantity;

                var reduceResult = existingStorageItem.ReduceQuantity(quantity);
                if (reduceResult.LocalizedError != null) return reduceResult.LocalizedError;

                return quantity;
            }
        }

        public GameResult<int> Remove(IEnumerable<Ingredient> ingredients)
        {
            int removedQuantity = 0;

            foreach (var ingredient in ingredients)
            {
                var containResult = Contains(ingredient.Item, ingredient.Quantity);

                if (containResult.LocalizedError != null && containResult.Result == true)
                {
                    return new LocalizedError
                    {
                        ErrorSeverity = ErrorSeverity.Normal,
                        EnglishMessage = "The storage does not have enough ingredients",
                        RussianMessage = "В хранилище недостаточно ингридиентов"
                    };
                }

                var removeResult = Remove(ingredient.Item, ingredient.Quantity);
                if (removeResult.LocalizedError != null) return removeResult.LocalizedError;

                removedQuantity += removeResult;
            }

            return removedQuantity;
        }


        public GameResult<bool> Contains(Item item, int quantity = 1)
        {
            var storageItemCreationResult = StorageItem.Create(new StorageItemCreationParams
            {
                Item = item,
                Quantity = quantity
            });
            if (storageItemCreationResult.LocalizedError != null) return storageItemCreationResult.LocalizedError;

            StorageItem storageItem = storageItemCreationResult;

            return Contains(storageItem);
        }

        public GameResult<bool> Contains(StorageItem storageItem)
        {
            if (storageItem.Quantity < 1)
            {
                return itemQuantityLowerThan1Error;
            }

            return items.Any(sItem => sItem == storageItem)
                || (items.Any(sItem => sItem.Item == storageItem.Item)
                    && items.Any(sItem => sItem.Quantity >= storageItem.Quantity));
        }


        public GameResult<int> TransferTo(Storage storage, Item item, int quantity = 1)
        {
            var storageItemCreationResult = StorageItem.Create(new StorageItemCreationParams
            {
                Item = item,
                Quantity = quantity
            });
            if (storageItemCreationResult.LocalizedError != null) return storageItemCreationResult.LocalizedError;

            StorageItem storageItem = storageItemCreationResult;

            return TransferTo(storage, storageItem);
        }

        public GameResult<int> TransferTo(Storage storage, StorageItem storageItem)
        {
            if (this == storage)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Can't transfer items to the same storage",
                    RussianMessage = "Нельзя переместить предмеВы в то же самое хранилище"
                };
            }

            var addResult = storage.Add(storageItem);
            if (addResult.LocalizedError != null) return addResult.LocalizedError;

            var removeResult = Remove(storageItem.Item, addResult);
            if (removeResult.LocalizedError != null) return removeResult.LocalizedError;

            return addResult;
        }

        public GameResult<int> TransferTo(Storage storage, IEnumerable<StorageItem> items)
        {
            int transferredQuantity = 0;

            foreach (var item in items)
            {
                if (!Items.Contains(item) || !Items.Any(storageItem => storageItem.Item == item.Item))
                {
                    return new LocalizedError
                    {
                        ErrorSeverity = ErrorSeverity.Critical,
                        EnglishMessage = $"{nameof(StorageItem)} {item.Item.Name}, id {item.Id} does not exist in the {nameof(Storage)}, id {Id}",
                        RussianMessage = $"{nameof(StorageItem)} {item.Item.Name} с id {item.Id} не существует в {nameof(Storage)} с id {Id}"
                    };
                }

                var transferToResult = TransferTo(storage, item);
                if (transferToResult.LocalizedError != null) return transferToResult.LocalizedError;

                transferredQuantity += transferToResult;
            }

            return transferredQuantity;
        }


        public GameResult<int> TransferEverythingTo(Storage storage, Item itemTypeToTransfer = null)
        {
            if (itemTypeToTransfer != null)
            {
                var itemsToTransfer = items.Where(storageItem => storageItem.Item == itemTypeToTransfer).ToList();
                return TransferTo(storage, itemsToTransfer);
            }
            else
            {
                return TransferTo(storage, items);
            }
        }

        public GameResult<StorageItem> GetItem(Item item)
        {
            var foundItem = items.FirstOrDefault(storageItem => storageItem.Item == item);
            if (foundItem is null)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "This item does not exist in the storage",
                    RussianMessage = "Этого предмета нет в хранилище"
                };
            }

            return foundItem;
        }

        public GameResult<decimal> AddCapacity(decimal amount)
        {
            Capacity += amount;

            return Capacity;
        }
        #endregion
    }
}
