using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Models.Common.Business;
using EnterpriseBot.Api.Models.Common.Crafting;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.Common.Essences;
using EnterpriseBot.Api.Models.Common.Storages;
using EnterpriseBot.Api.Models.Contexts;
using EnterpriseBot.Api.Models.ModelCreationParams.Business;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings.GameplaySettings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using EnterpriseBot.BackgroundJobs.Jobs;
using EnterpriseBot.BackgroundJobs.Params;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EnterpriseBot.Api.Utils.Constants;
using static EnterpriseBot.Api.Utils.Miscellaneous;

namespace EnterpriseBot.Api.Areas.Business
{
    [Area(nameof(Business))]
    public class CompanyController : Controller, IGameController<Company>
    {
        private readonly ApplicationContext ctx;
        private readonly GameplaySettings gameplaySettings;
        private readonly LocalizationSettings localizationSettings;

        private readonly LocalizationSetting modelLocalization;

        public CompanyController(ApplicationContext context,
                             IOptions<GameplaySettings> gameplayOptions,
                             IOptions<LocalizationSettings> localizationOptions)
        {
            this.ctx = context;
            this.gameplaySettings = gameplayOptions.Value;
            this.localizationSettings = localizationOptions.Value;

            modelLocalization = localizationSettings.Business.Company;
        }

        ///<inheritdoc/>
        [HttpPost]
        public async Task<GameResult<Company>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            Company company = await ctx.Companies.FindAsync(id);
            //if (company == null) return CompanyDoesNotExist(id);

            return company;
        }

        /////<inheritdoc/>
        [HttpPost]
        public async Task<GameResult<Company>> Create([FromBody] CompanyCreationParams cp/*, [FromServices] PostgresTransactionLimiter postgresTransactionLimiter*/)
        {
            return await CreateCompany(cp/*, postgresTransactionLimiter*/);
        }

        /// <summary>
        /// Withdraws units from the future general manager and creates a new company
        /// </summary>
        /// <returns>Created company</returns>
        /// <remarks>Customer is a general manager, so there's no need to specify one separately</remarks>
        [HttpPost]
        public async Task<GameResult<Company>> Buy([FromBody] CompanyCreationParams cp)
        {
            Player generalManager = await ctx.Players.FindAsync(cp.GeneralManagerId);
            if (generalManager == null) return Errors.DoesNotExist(cp.GeneralManagerId, localizationSettings.Essences.Player);

            decimal companyPrice = gameplaySettings.Prices.Company.DefaultPrice;
            if (generalManager.Units < companyPrice)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Not enough units to create a company",
                    RussianMessage = "Недостаточно юнитов для создания компании"
                };
            }

            generalManager.Units -= companyPrice;

            return await CreateCompany(cp);
        }

        /// <summary>
        /// Changes company owner
        /// </summary>
        /// <returns>New owner</returns>
        [HttpPost]
        public async Task<EmptyGameResult> ChangeOwner([FromBody] string json)
        {
            var pars = new
            {
                companyId = default(long),
                newOwnerId = default(long)
            };
            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Company company = await ctx.Companies.FindAsync(d.companyId);
            if (company == null) return CompanyDoesNotExist(d.companyId);

            Player newOwner = await ctx.Players.FindAsync(d.newOwnerId);
            if (newOwner == null) return Errors.DoesNotExist(d.newOwnerId, localizationSettings.Essences.Player);

            if (company.GeneralManagerId == newOwner.Id)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Can’t transfer company ownership to yourself",
                    RussianMessage = "Нельзя передать компанию себе самому"
                };
            }

            company.GeneralManager = newOwner;

            await ctx.SaveChangesAsync();

            return new EmptyGameResult();
        }

        /// <summary>
        /// Add output item to the company output items list
        /// </summary>
        /// <returns>Output item list</returns>
        [HttpPost]
        public async Task<GameResult<IEnumerable<Item>>> AddOutputItem([FromBody] string json)
        {
            var pars = new
            {
                companyId = default(long),
                itemId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Company company = await ctx.Companies.FindAsync(d.companyId);
            if (company == null) return CompanyDoesNotExist(d.companyId);

            Item item = await ctx.Items.FindAsync(d.itemId);
            if (item == null) return Errors.DoesNotExist(d.itemId, localizationSettings.Crafting.Item);

            if (company.OutputItems.Contains(item))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "The company already has this item in the output items list",
                    RussianMessage = "В списке предметов, которые производит компания, уже есть этот предмет"
                };
            }

            if (company.OutputItems == null)
            {
                company.OutputItems = new List<Item> { item };
                await ctx.SaveChangesAsync();

                return company.OutputItems;
            }

            if (company.OutputItems.Count < gameplaySettings.Company.MaxOutputItems)
            {
                company.OutputItems.Add(item);
                await ctx.SaveChangesAsync();

                return (await ctx.Companies.FindAsync(d.companyId)).OutputItems;
            }
            else
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = $"More output items are not allowed. Max output items quantity: {gameplaySettings.Company.MaxOutputItems}",
                    RussianMessage = $"Нельзя добавить больше выходных предметов. Максимальное количество: {gameplaySettings.Company.MaxOutputItems}"
                };
            }
        }

        /// <summary>
        /// Remove output item from the company output items list
        /// </summary>
        /// <param name="companyId">Id of the company to remove output item</param>
        /// <param name="itemInOutput_Id">Item id in the company output items list to remove</param>
        /// <returns>Output items list</returns>
        [HttpPost]
        public async Task<GameResult<IEnumerable<Item>>> RemoveOutputItem([FromBody] string json)
        {
            var pars = new
            {
                companyId = default(long),
                itemInOutputId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Company company = await ctx.Companies.FindAsync(d.companyId);
            if (company == null) return CompanyDoesNotExist(d.companyId);

            if (company.OutputItems == null)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Output items list is empty",
                    RussianMessage = "Список предметов, которые производит компания, пуст"
                };
            }

            Item item = company.OutputItems.Where(m => m.Id == d.itemInOutputId).FirstOrDefault();

            if (item == null)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "There's no such an item in output items",
                    RussianMessage = "Такого предмета нет в списке предметов, которые производит компания"
                };
            }

            company.OutputItems.Remove(item);
            await ctx.SaveChangesAsync();

            return (await ctx.Companies.FindAsync(d.companyId)).OutputItems;
        }

        /// <summary>
        /// Withdraws units from a company owner account and adds them to the company account
        /// </summary>
        /// <returns>Company units</returns>
        [HttpPost]
        public async Task<GameResult<decimal>> AddUnitsFromGeneralManager([FromBody] string json)
        {
            var pars = new
            {
                companyId = default(long),
                amount = default(decimal)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Company company = await ctx.Companies.FindAsync(d.companyId);
            if (company == null) return CompanyDoesNotExist(d.companyId);

            var player = company.GeneralManager;
            if (player == null)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Critical,
                    EnglishMessage = "Company must have general manager",
                    RussianMessage = "У компании должен быть генеральный директор"
                };
            }

            if (d.amount < 0)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "It is forbidden to send a negative amount of units",
                    RussianMessage = "Нельзя отправлять отрицательное количество юнитов"
                };
            }

            if (player.Units - d.amount < 0)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Not enough units",
                    RussianMessage = "Недостаточно юнитов"
                };
            }

            player.Units -= d.amount;
            company.CompanyUnits += d.amount;

            await ctx.SaveChangesAsync();
            decimal units = (await ctx.Companies.FindAsync(d.companyId)).CompanyUnits;

            return units;
        }

        /// <summary>
        /// Withdraws company units to the owner's account
        /// </summary>
        /// <returns>Company units</returns>
        [HttpPost]
        public async Task<GameResult<decimal>> WithdrawUnits([FromBody] string json)
        {
            var pars = new
            {
                companyId = default(long),
                amount = default(decimal)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Company company = await ctx.Companies.FindAsync(d.companyId);
            if (company == null) return CompanyDoesNotExist(d.companyId);

            Player player = company.GeneralManager;
            if (player == null)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Critical,
                    EnglishMessage = "Company must have general manager",
                    RussianMessage = "У компании должен быть генеральный директор"
                };
            }

            if (d.amount == 0)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Unable to send 0 units",
                    RussianMessage = "Нельзя отправить 0 юнитов"
                };
            }
            if (d.amount < 0)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Unable to send negative amount of units",
                    RussianMessage = "Нельзя отправить отрицательное количество юнитов"
                };
            }
            if (company.CompanyUnits - d.amount < 0)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Company can not have a negative amount of units",
                    RussianMessage = "Компания не может иметь отрицательное количество юнитов"
                };
            }

            company.CompanyUnits -= d.amount;
            player.Units += d.amount;
            await ctx.SaveChangesAsync();

            decimal companyUnits = (await ctx.Companies.FindAsync(d.companyId)).CompanyUnits;

            return companyUnits;
        }

        /// <summary>
        /// Adds units to the company
        /// </summary>
        /// <returns>Company units</returns>
        [HttpPost]
        public async Task<GameResult<decimal>> AddUnits([FromBody] string json)
        {
            var pars = new
            {
                companyId = default(long),
                amount = default(decimal)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Company company = await ctx.Companies.FindAsync(d.companyId);
            if (company == null) return CompanyDoesNotExist(d.companyId);

            if (d.amount < 0)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Critical,
                    EnglishMessage = "It is forbidden to add a negative amount of units",
                    RussianMessage = "Нельзя добавлять отрицательное количество юнитов"
                };
            }

            company.CompanyUnits += d.amount;

            await ctx.SaveChangesAsync();

            decimal units = (await ctx.Companies.FindAsync(d.companyId)).CompanyUnits;

            return units;
        }

        /// <summary>
        /// Diminishes company units by specified amount
        /// </summary>
        /// <returns>Company units</returns>
        [HttpPost]
        public async Task<GameResult<decimal>> DiminishUnits([FromBody] string json)
        {
            var pars = new
            {
                companyId = default(long),
                amount = default(decimal)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Company company = await ctx.Companies.FindAsync(d.companyId);
            if (company == null) return CompanyDoesNotExist(d.companyId);

            if (d.amount < 0)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Critical,
                    EnglishMessage = "It is forbidden to remove a negative amount of units",
                    RussianMessage = "Нельзя отнимать отрицательное количество юнитов"
                };
            }
            if (company.CompanyUnits - d.amount < 0)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Critical,
                    EnglishMessage = $"Company with id {d.companyId} doesn't have enough units",
                    RussianMessage = $"У компании с id {d.companyId} недостаточно юнитов"
                };
            }

            company.CompanyUnits -= d.amount;

            await ctx.SaveChangesAsync();

            decimal units = (await ctx.Companies.FindAsync(d.companyId)).CompanyUnits;

            return units;
        }

        /// <summary>
        /// Changes company description
        /// </summary>
        /// <param name="companyId">Company id which description to change</param>
        /// <param name="newDesc">New description</param>
        /// <returns>New company description</returns>
        [HttpPost]
        public async Task<GameResult<string>> ChangeDescription([FromBody] string json)
        {
            var pars = new
            {
                companyId = default(long),
                newDescription = default(string)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Company company = await ctx.Companies.FindAsync(d.companyId);
            if (company == null) return CompanyDoesNotExist(d.companyId);

            if (!CheckDescription(d.newDescription))
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Company description has not passed verification. Please check the input and try later",
                    RussianMessage = "Описание компании не прошло проверку. Пожалуйста, проверьте ввод и попробуйте ещё раз"
                };
            }

            company.Description = d.newDescription;

            await ctx.SaveChangesAsync();

            return company.Description;
        }

        /// <summary>
        /// Adds a truck to the company truck garage
        /// </summary>
        /// <returns>New truck</returns>
        [HttpPost]
        public async Task<GameResult<Truck>> AddTruck([FromBody] TruckCreationParams cp)
        {
            return await CreateTruck(cp);
        }

        /// <summary>
        /// Decreases company units and adds new truck with default properties to the company truck garage
        /// </summary>
        /// <returns>New truck</returns>
        [HttpPost]
        public async Task<GameResult<Truck>> BuyTruck([FromBody] string json)
        {
            var pars = new
            {
                companyId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Company company = await ctx.Companies.FindAsync(d.companyId);
            if (company == null) return CompanyDoesNotExist(d.companyId);

            decimal truckPrice = gameplaySettings.Prices.Truck.DefaultPrice;

            if (company.CompanyUnits < truckPrice)
            {
                return new LocalizedError
                {
                    ErrorSeverity = ErrorSeverity.Normal,
                    EnglishMessage = "Not enough units",
                    RussianMessage = "Недостаточно юнитов"
                };
            }

            company.CompanyUnits -= truckPrice;

            TruckCreationParams cp = new TruckCreationParams
            {
                TruckGarageId = company.TruckGarage.Id,
                Capacity = gameplaySettings.Storages.Trunk.DefaultCapacity,
                DeliveringSpeedInSeconds = gameplaySettings.Truck.DefaultTime
            };

            return await CreateTruck(cp);
        }

        /// <summary>
        /// Sends a truck to deliver items to another business
        /// </summary>
        [HttpPost]
        public async Task<EmptyGameResult> SendTruck([FromBody] string json, [FromServices] IBackgroundJobClient backgroundJobClient)
        {
            var pars = new
            {
                truckId = default(long),
                contractId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Contract contract = await ctx.Contracts.FindAsync(d.contractId);
            if (contract == null) return Errors.DoesNotExist(d.contractId, localizationSettings.Business.Contract);

            IncomeStorage incomeStorage;

            switch (contract.ContractIncomeBusinessType)
            {
                case BusinessType.Company:
                    incomeStorage = contract.IncomeCompany.IncomeStorage;
                    break;

                case BusinessType.Shop:
                    incomeStorage = contract.IncomeShop.IncomeStorage;
                    break;

                default:
                    return new LocalizedError
                    {
                        ErrorSeverity = ErrorSeverity.Critical,
                        EnglishMessage = $"Unknown {nameof(BusinessType)} value",
                        RussianMessage = $"Неизвестное значение {nameof(BusinessType)}"
                    };
            }

            Truck truck = await ctx.Trucks.FindAsync(d.truckId);
            if (truck == null) return Errors.DoesNotExist(d.truckId, localizationSettings.Business.Truck);

            var jobParams = new UnloadTruckJobParams
            {
                ContractId = d.contractId,
                TruckId = d.truckId,
                IncomeStorageId = incomeStorage.Id
            };
            int delay = truck.DeliveringSpeedInSeconds / 2;
            string unloadTruckJobId = backgroundJobClient.Schedule<UnloadTruckJob>(j => j.Execute(jobParams), delay: TimeSpan.FromSeconds(delay));

            truck.UnloadTruckJobId = unloadTruckJobId;

            truck.CurrentState = TruckState.OnTheWayTowards;

            return new EmptyGameResult();
        }

        /// <summary>
        /// Immediately returns truck back
        /// </summary>
        [HttpPost]
        public async Task<EmptyGameResult> ReturnTruck([FromBody] string json)
        {
            var pars = new
            {
                truckId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Truck truck = await ctx.Trucks.FindAsync(d.truckId);
            if (truck == null) return Errors.DoesNotExist(d.truckId, localizationSettings.Business.Truck);

            truck.ReturnTruckJobId = null;

            truck.CurrentState = TruckState.ReadyToGo;

            await ctx.SaveChangesAsync();

            return new EmptyGameResult();
        }

        /// <summary>
        /// Returns a company which name matches the specified one
        /// </summary>
        /// <returns>Company which name matches the specified one</returns>
        [HttpPost]
        public async Task<GameResult<Company>> GetByName([FromBody] string json)
        {
            var pars = new
            {
                name = default(string)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            Company company = await ctx.Companies.FirstOrDefaultAsync(c => c.Name.ToLower().Trim() == d.name.ToLower().Trim());
            //if (company == null) return CompanyDoesNotExist(d.name);

            return company;
        }


        [NonAction]
        private LocalizedError CompanyDoesNotExist(long id)
        {
            return Errors.DoesNotExist(id, modelLocalization);
        }

        [NonAction]
        private async Task<GameResult<Company>> CreateCompany(CompanyCreationParams cp/*, PostgresTransactionLimiter postgresTransactionLimiter*/)
        {
            long createdCompanyId;

            //using (var pgTransaction = await postgresTransactionLimiter.WaitOtherTransactionsAndBeginAsync(ctx))
            {
                //var transaction = pgTransaction.Transaction;

                UserInputRequirements req = localizationSettings.UserInputRequirements;

                if (!CheckBusinessName(cp.Name))
                {
                    return new LocalizedError
                    {
                        ErrorSeverity = ErrorSeverity.Normal,
                        EnglishMessage = "Company name has not passed verification. " + string.Format(req.Name.English, BusinessNameMaxLength),
                        RussianMessage = "Название компании не прошло проверку. " + string.Format(req.Name.Russian, BusinessNameMaxLength)
                    };
                }
                if (!CheckDescription(cp.Description))
                {
                    return new LocalizedError
                    {
                        ErrorSeverity = ErrorSeverity.Normal,
                        EnglishMessage = "Company description has not passed verification. " + string.Format(req.Description.English, DescriptionMaxLength),
                        RussianMessage = "Описание компании не прошло проверку. " + string.Format(req.Description.Russian, DescriptionMaxLength)
                    };
                }

                //can be optimized by removing "ToList", which is not recommended (won't generate request with Trim and string.Equals)
                if ((await ctx.Companies.ToListAsync()).Any(company => CompareNames(company.Name, cp.Name))
                    || (await ctx.Shops.ToListAsync()).Any(shop => CompareNames(shop.Name, cp.Name)))
                {
                    return new LocalizedError
                    {
                        ErrorSeverity = ErrorSeverity.Normal,
                        EnglishMessage = "A company or a shop with the same name already exists",
                        RussianMessage = "Компания или магазин с таким названием уже существует"
                    };
                }

                Player generalManager = await ctx.Players.FindAsync(cp.GeneralManagerId);
                if (generalManager == null) return Errors.DoesNotExist(cp.GeneralManagerId, localizationSettings.Essences.Player);

                IncomeStorage createdIncomeStorage = (await ctx.IncomeStorages.AddAsync(
                    new IncomeStorage
                    {
                        Capacity = gameplaySettings.Storages.Income.DefaultCapacity
                    })).Entity;

                OutcomeStorage createdOutcomeStorage = (await ctx.OutcomeStorages.AddAsync(
                    new OutcomeStorage
                    {
                        Capacity = gameplaySettings.Storages.Outcome.DefaultCapacity
                    })).Entity;

                WorkerStorage createdWorkerStorage = (await ctx.WorkerStorages.AddAsync(
                    new WorkerStorage
                    {
                        Capacity = gameplaySettings.Storages.Worker.DefaultCapacity
                    })).Entity;

                TruckGarage createdTruckGarage = (await ctx.TruckGarages.AddAsync(
                    new TruckGarage
                    {
                        Capacity = gameplaySettings.TruckGarage.DefaultCapacity
                    })).Entity;

                Company createdCompany = (await ctx.Companies.AddAsync(
                    new Company
                    {
                        Name = cp.Name,
                        Description = cp.Description,
                        GeneralManager = generalManager,
                        CompanyUnits = gameplaySettings.Company.DefaultUnits,
                        IncomeStorage = createdIncomeStorage,
                        OutcomeStorage = createdOutcomeStorage,
                        WorkerStorage = createdWorkerStorage,
                        TruckGarage = createdTruckGarage
                    })).Entity;

                //await ctx.SaveChangesAsync();

                //adding first truck
                TrunkStorage firstTruckTrunk = (await ctx.TrunkStorages.AddAsync(
                    new TrunkStorage
                    {
                        Capacity = gameplaySettings.Storages.Trunk.DefaultCapacity
                    })).Entity;

                Truck firstTruck = (await ctx.Trucks.AddAsync(
                    new Truck
                    {
                        DeliveringSpeedInSeconds = gameplaySettings.Truck.DefaultTime,
                        TruckGarage = createdTruckGarage,
                        TrunkStorage = firstTruckTrunk
                    })).Entity;

                await ctx.SaveChangesAsync();
                //await transaction.CommitAsync();
                createdCompanyId = createdCompany.Id;
            }

            return await ctx.Companies.FindAsync(createdCompanyId);
        }

        [NonAction]
        private async Task<GameResult<Truck>> CreateTruck(TruckCreationParams cp)
        {
            TruckGarage truckGarage = await ctx.TruckGarages.FindAsync(cp.TruckGarageId);
            if (truckGarage == null) return Errors.DoesNotExist(cp.TruckGarageId, localizationSettings.Business.TruckGarage);

            TrunkStorage createdTrunkStorage = (await ctx.TrunkStorages.AddAsync(
                new TrunkStorage
                {
                    Capacity = cp.Capacity ?? gameplaySettings.Storages.Trunk.DefaultCapacity
                })).Entity;

            Truck createdTruck = (await ctx.Trucks.AddAsync(
                new Truck
                {
                    TruckGarage = truckGarage,
                    TrunkStorage = createdTrunkStorage,
                    DeliveringSpeedInSeconds = cp.DeliveringSpeedInSeconds ?? gameplaySettings.Truck.DefaultTime,
                    CurrentState = TruckState.ReadyToGo
                })).Entity;

            await ctx.SaveChangesAsync();

            return await ctx.Trucks.FindAsync(createdTruck.Id);
        }
    }
}