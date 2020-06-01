using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Models.Common.Business;
using EnterpriseBot.Api.Models.Common.Crafting;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.Common.Storages;
using EnterpriseBot.Api.Models.Contexts;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings.GameplaySettings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using EnterpriseBot.BackgroundJobs.Jobs;
using EnterpriseBot.BackgroundJobs.Params;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Areas.Storages
{
    [Area(nameof(Storages))]
    public class IncomeStorageController : Controller, IGameController<IncomeStorage>
    {
        private readonly ApplicationContext ctx;
        private readonly GameplaySettings gameplaySettings;
        private readonly LocalizationSettings localizationSettings;

        private readonly LocalizationSetting modelLocalization;

        public IncomeStorageController(ApplicationContext context,
                                IOptions<GameplaySettings> gameplayOptions,
                                IOptions<LocalizationSettings> localizationOptions)
        {
            this.ctx = context;
            this.gameplaySettings = gameplayOptions.Value;
            this.localizationSettings = localizationOptions.Value;

            modelLocalization = localizationSettings.Storages.IncomeStorage;
        }

        /// <inheritdoc/>
        [HttpPost]
        public async Task<GameResult<IncomeStorage>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            IncomeStorage incomeStorage = await ctx.IncomeStorages.FindAsync(id);
            //if (incomeStorage == null) return IncomeStorageDoesNotExist(id);

            return incomeStorage;
        }

        //[HttpPost]
        //public async Task<GameResult<IncomeStorage>> Create([FromBody] IncomeStorageCreationParams cp)
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

            IncomeStorage incomeStorage = await ctx.IncomeStorages.FindAsync(d.incomeStorageId);
            if (incomeStorage == null) return IncomeStorageDoesNotExist(d.incomeStorageId);

            Item item = await ctx.Items.FindAsync(d.itemId);
            if (item == null) return Errors.DoesNotExist(d.itemId, localizationSettings.Crafting.Item);

            if (incomeStorage.Items == null)
                incomeStorage.Items = new List<StorageItem>();

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

            incomeStorage.Items.Add(new StorageItem
            {
                Item = item,
                Quantity = d.quantity
            });

            await ctx.SaveChangesAsync();

            return (await ctx.IncomeStorages.FindAsync(d.incomeStorageId)).Items;
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

            IncomeStorage incomeStorage = await ctx.IncomeStorages.FindAsync(d.incomeStorageId);
            if (incomeStorage == null) return IncomeStorageDoesNotExist(d.incomeStorageId);

            Item item = await ctx.Items.FindAsync(d.itemId);
            if (item == null) return Errors.DoesNotExist(d.itemId, localizationSettings.Crafting.Item);

            if (incomeStorage.Items == null || !incomeStorage.Items.Any(storageItem => storageItem.Item == item))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The storage does not have this item",
                    RussianMessage = "На складе нет такого предмета"
                };
            }

            StorageItem itemInStorage = incomeStorage.Items.Single(storageItem => storageItem.Item == item);

            if (itemInStorage.Quantity <= d.quantity)
            {
                ctx.StorageItems.Remove(itemInStorage);
            }
            else
            {
                itemInStorage.Quantity -= d.quantity;
            }

            await ctx.SaveChangesAsync();

            return (await ctx.IncomeStorages.FindAsync(d.incomeStorageId)).Items;
        }

        /// <summary>
        /// Moves an item to worker storage
        /// </summary>
        /// <returns>Income storage instance</returns>
        [HttpPost]
        public async Task<GameResult<IncomeStorage>> MoveToWorkerStorage([FromBody] string json)
        {
            var pars = new
            {
                incomeStorageId = default(long),
                workerStorageId = default(long),
                itemId = default(long),
                quantity = default(int)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            IncomeStorage incomeStorage = await ctx.IncomeStorages.FindAsync(d.incomeStorageId);
            if (incomeStorage == null) return IncomeStorageDoesNotExist(d.incomeStorageId);

            WorkerStorage workerStorage = await ctx.WorkerStorages.FindAsync(d.workerStorageId);
            if (workerStorage == null) return Errors.DoesNotExist(d.workerStorageId, localizationSettings.Storages.WorkerStorage);

            Item item = await ctx.Items.FindAsync(d.itemId);
            if (item == null) return Errors.DoesNotExist(d.itemId, localizationSettings.Crafting.Item);

            if (d.quantity <= 0)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Item quantity can not be lower than or equal to 0",
                    RussianMessage = "Количество предметов не может быть меньше или равно 0"
                };
            }

            if (incomeStorage.Items == null || !incomeStorage.Items.Any(storageItem => storageItem.Item == item))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The storage does not have this item",
                    RussianMessage = "На складе нет такого предмета"
                };
            }

            var currentSpaceOccupied = workerStorage.Items.Sum(storageItem => storageItem.Quantity * storageItem.Item.Space);
            if (workerStorage.Capacity < currentSpaceOccupied + (d.quantity * item.Space))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The storage doesn't have enough space",
                    RussianMessage = "На складе недостаточно места"
                };
            }

            StorageItem itemInStorage = incomeStorage.Items.Single(storageItem => storageItem.Item == item);

            if (itemInStorage.Quantity <= d.quantity)
            {
                ctx.StorageItems.Remove(itemInStorage);
            }
            else
            {
                itemInStorage.Quantity -= d.quantity;
            }

            var itemInDestinationStorage = workerStorage.Items.Where(storageItem => storageItem.Item == item).FirstOrDefault();

            if (itemInDestinationStorage == null)
            {
                itemInDestinationStorage = new StorageItem
                {
                    Item = item,
                    Quantity = d.quantity
                };

                workerStorage.Items.Add(itemInDestinationStorage);
            }
            else
            {
                itemInDestinationStorage.Quantity += d.quantity;
            }


            await ctx.SaveChangesAsync();

            return await ctx.IncomeStorages.FindAsync(d.incomeStorageId);
        }

        /// <summary>
        /// Moves an item to showcase storage
        /// </summary>
        /// <returns>Income storage instance</returns>
        [HttpPost]
        public async Task<GameResult<IncomeStorage>> MoveToShowcaseStorage([FromBody] string json)
        {
            var pars = new
            {
                incomeStorageId = default(long),
                showcaseStorageId = default(long),
                itemId = default(long),
                quantity = default(int),
                price = default(decimal)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            IncomeStorage incomeStorage = await ctx.IncomeStorages.FindAsync(d.incomeStorageId);
            if (incomeStorage == null) return IncomeStorageDoesNotExist(d.incomeStorageId);

            ShowcaseStorage showcaseStorage = await ctx.ShowcaseStorages.FindAsync(d.showcaseStorageId);
            if (showcaseStorage == null) return Errors.DoesNotExist(d.showcaseStorageId, localizationSettings.Storages.ShowcaseStorage);

            Item item = await ctx.Items.FindAsync(d.itemId);
            if (item == null) return Errors.DoesNotExist(d.itemId, localizationSettings.Crafting.Item);

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
            if (incomeStorage.OwningShop != showcaseStorage.OwningShop)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Critical,
                    EnglishMessage = $"A shop that owns {localizationSettings.Storages.IncomeStorage.English} " +
                                     $"with id {incomeStorage.Id} and the shop that owns " +
                                     $"{localizationSettings.Storages.ShowcaseStorage.English} with id {showcaseStorage.Id} are not the same",

                    RussianMessage = $"Магазин, который владеет {localizationSettings.Storages.IncomeStorage.Russian} " +
                                     $"с id {incomeStorage.Id} и магазин, который владеет " +
                                     $"{localizationSettings.Storages.ShowcaseStorage.Russian} с id {showcaseStorage.Id} - разные"
                };
            }

            if (incomeStorage.Items == null || !incomeStorage.Items.Any(storageItem => storageItem.Item == item))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The storage does not have this item",
                    RussianMessage = "На складе нет такого предмета"
                };
            }

            var currentSpaceOccupied = showcaseStorage.Items.Sum(storageItem => storageItem.StorageItem.Quantity * storageItem.StorageItem.Item.Space);
            if (showcaseStorage.Capacity < currentSpaceOccupied + (d.quantity * item.Space))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The storage doesn't have enough space",
                    RussianMessage = "На складе недостаточно места"
                };
            }

            StorageItem itemInStorage = incomeStorage.Items.Single(storageItem => storageItem.Item == item);

            if (itemInStorage.Quantity <= d.quantity)
            {
                ctx.StorageItems.Remove(itemInStorage);
            }
            else
            {
                itemInStorage.Quantity -= d.quantity;
            }

            StorageItemWithPrice itemInDestinationStorage = showcaseStorage.Items.Where(storageItem => storageItem.StorageItem.Item == item).FirstOrDefault();

            if (itemInDestinationStorage == null)
            {
                itemInDestinationStorage = new StorageItemWithPrice
                {
                    StorageItem = new StorageItem
                    {
                        Item = item,
                        Quantity = d.quantity
                    },
                    Price = d.price
                };

                showcaseStorage.Items.Add(itemInDestinationStorage);
            }
            else
            {
                itemInDestinationStorage.StorageItem.Quantity += d.quantity;
            }


            await ctx.SaveChangesAsync();

            return await ctx.IncomeStorages.FindAsync(d.incomeStorageId);
        }

        /// <summary>
        /// Unloads the truck to the storage
        /// </summary>
        [HttpPost]
        public async Task<EmptyGameResult> UnloadTruck([FromBody] string json, [FromServices] IBackgroundJobClient backgroundJobClient)
        {
            async Task ScheduleReturnAndSave(Truck truck, IBackgroundJobClient jobClient)
            {
                var returnTruckJobParams = new ReturnTruckJobParams
                {
                    TruckId = truck.Id
                };

                int delay = truck.DeliveringSpeedInSeconds / 2;
                string returnTruckJobId = backgroundJobClient.Schedule<ReturnTruckJob>(j => j.Execute(returnTruckJobParams), delay: TimeSpan.FromSeconds(delay));
                truck.ReturnTruckJobId = returnTruckJobId;

                if (truck.UnloadTruckJobId != null)
                {
                    backgroundJobClient.Delete(truck.UnloadTruckJobId);
                    truck.UnloadTruckJobId = null;
                }

                truck.CurrentState = TruckState.OnTheWayBack;

                await ctx.SaveChangesAsync();
            }
            void UnloadItem(IncomeStorage incomeStorage, TrunkStorage trunkStorage, StorageItem itemInTrunk, int quantity, Contract contract)
            {
                if (itemInTrunk.Quantity <= quantity)
                    ctx.StorageItems.Remove(itemInTrunk);
                else
                    itemInTrunk.Quantity -= quantity;

                StorageItem itemInIncome = incomeStorage.Items.Where(storageItem => storageItem.Item == itemInTrunk.Item).FirstOrDefault();
                if (itemInIncome == null)
                {
                    incomeStorage.Items.Add(new StorageItem
                    {
                        Item = itemInTrunk.Item,
                        Quantity = quantity
                    });
                }
                else
                {
                    itemInIncome.Quantity += quantity;
                }

                contract.DeliveredAmount += (uint)quantity;
            }
            void UnloadAllItems(IncomeStorage incomeStorage, TrunkStorage trunkStorage, Contract contract)
            {
                for (int i = trunkStorage.Items.Count() - 1; i >= 0; i--)
                {
                    StorageItem currentItem = trunkStorage.Items[i];
                    UnloadItem(incomeStorage, trunkStorage, currentItem, currentItem.Quantity, contract);
                }
            }

            var pars = new
            {
                incomeStorageId = default(long),
                truckId = default(long),
                contractId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            IncomeStorage incomeStorage = await ctx.IncomeStorages.FindAsync(d.incomeStorageId);
            if (incomeStorage == null) return IncomeStorageDoesNotExist(d.incomeStorageId);

            Truck truck = await ctx.Trucks.FindAsync(d.truckId);
            if (truck == null) return Errors.DoesNotExist(d.truckId, localizationSettings.Business.Truck);

            Contract contract = await ctx.Contracts.FindAsync(d.contractId);
            if (contract == null) return Errors.DoesNotExist(d.contractId, localizationSettings.Business.Contract);

            if (truck.TrunkStorage.Items == null)
            {
                await ScheduleReturnAndSave(truck, backgroundJobClient);

                return new EmptyGameResult();
            }

            TrunkStorage trunkStorage = truck.TrunkStorage;

            int currentSpaceOccupiedInIncomeStorage = incomeStorage.Items.Sum(storageItem => storageItem.Quantity * storageItem.Item.Space);
            int currentSpaceOccupiedInTrunkStorage = trunkStorage.Items.Sum(storageItem => storageItem.Quantity * storageItem.Item.Space);

            //if so, unload as many items as possible
            if (incomeStorage.Capacity < currentSpaceOccupiedInIncomeStorage + currentSpaceOccupiedInTrunkStorage)
            {
                int availableStorage = incomeStorage.Capacity - currentSpaceOccupiedInIncomeStorage;
                for (int i = trunkStorage.Items.Count() - 1; i >= 0; i--) //there might be enough space for multiple items to unload
                {
                    int leastItemSpace = trunkStorage.Items.Min(storageItem => storageItem.Item.Space);
                    if (availableStorage < leastItemSpace)
                    {
                        break;
                    }

                    StorageItem itemWithLeastSpace = trunkStorage.Items.FirstOrDefault(storageItem => storageItem.Item.Space == leastItemSpace);
                    int unloadQuantity = (int)Math.Floor((decimal)(availableStorage / leastItemSpace));
                    UnloadItem(incomeStorage, trunkStorage, itemWithLeastSpace, unloadQuantity, contract);

                    currentSpaceOccupiedInIncomeStorage = incomeStorage.Items.Sum(storageItem => storageItem.Quantity * storageItem.Item.Space);
                    //currentSpaceOccupiedInTrunkStorage = trunkStorage.Items.Sum(storageItem => storageItem.Quantity * storageItem.Item.Space); //there's no need to update this
                    availableStorage = incomeStorage.Capacity - currentSpaceOccupiedInIncomeStorage;
                }

                await ScheduleReturnAndSave(truck, backgroundJobClient);

                return new EmptyGameResult();
            }

            UnloadAllItems(incomeStorage, trunkStorage, contract);

            await ScheduleReturnAndSave(truck, backgroundJobClient);

            return new EmptyGameResult();
        }


        [NonAction]
        private LocalizedError IncomeStorageDoesNotExist(long id)
        {
            return Errors.DoesNotExist(id, modelLocalization);
        }
    }
}
