using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Attributes;
using EnterpriseBot.Api.Game.Business.Company;
using EnterpriseBot.Api.Game.Storages;
using EnterpriseBot.Api.Models.ApiCreationParams.Business;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.Contexts;
using EnterpriseBot.Api.Models.ModelCreationParams.Business;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using EnterpriseBot.BackgroundJobs.Abstractions;
using EnterpriseBot.BackgroundJobs.Jobs;
using EnterpriseBot.BackgroundJobs.Params;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Areas.Business.Company.Controllers
{
    [Area(nameof(Business))]
    [SubArea(nameof(Business.Company))]
    public class TruckController : Controller,
                                   IGameController<Truck, TruckApiCreationParams>
    {
        private readonly ApplicationContext ctx;
        private readonly ILogger<TruckController> logger;

        private readonly GameSettings gameSettings;

        private readonly LocalizationSettings localization;
        private readonly LocalizationSetting modelLocalization;

        public TruckController(ApplicationContext dbContext,
                               ILogger<TruckController> logger,
                               IOptionsSnapshot<GameSettings> gameOptionsAccessor)
        {
            this.ctx = dbContext;
            this.logger = logger;

            this.gameSettings = gameOptionsAccessor.Value;

            this.localization = this.gameSettings.Localization;
            this.modelLocalization = this.localization.Business.Company.Truck;
        }

        ///<inheritdoc/>
        public async Task<GameResult<Truck>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            var model = await ctx.Trucks.FindAsync(id);

            return model;
        }

        public async Task<GameResult<Truck>> Create([FromBody] TruckApiCreationParams pars)
        {
            var truckGarage = await ctx.TruckGarages.FindAsync(pars.TruckGarageId);
            if (truckGarage == null) return Errors.DoesNotExist(pars.TruckGarageId, localization.Business.Company.TruckGarage);

            var creationResult = Truck.Create(new TruckCreationParams
            {
                TruckGarage = truckGarage,
                Capacity = pars.Capacity,
                DeliveringSpeedInSeconds = pars.DeliveringSpeedInSeconds
            });
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.Trucks.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }


        public async Task<EmptyGameResult> Send([FromBody] string json, [FromServices] IBackgroundJobScheduler jobs)
        {
            var pars = new
            {
                modelId = default(long),
                contractId = default(long),
                invokerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var truck = await ctx.Trucks.FindAsync(d.modelId);
            if (truck == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var contract = await ctx.CompanyContracts.FindAsync(d.contractId);
            if (contract == null) return Errors.DoesNotExist(d.contractId, localization.Business.Company.CompanyContract);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);



            var actionResult = truck.Send(contract, invoker);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;


            var getSpaceResult = truck.GetSpaceOccupiedByItemsForContract(contract);
            if (getSpaceResult.LocalizedError != null) return getSpaceResult.LocalizedError;

            decimal space = getSpaceResult;

            var getStorageResult = contract.IncomeCompany.GetCompanyStorageWithAvailableSpace(space, CompanyStorageType.Income);
            if (getStorageResult.LocalizedError != null) return getStorageResult.LocalizedError;

            CompanyStorage storage = getStorageResult;

            ScheduleUnloadTruckJob(truck, storage, contract, jobs);

            await ctx.SaveChangesAsync();

            return new EmptyGameResult();
        }

        public async Task<EmptyGameResult> Return([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var truck = await ctx.Trucks.FindAsync(d.modelId);
            if (truck == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = truck.Return();
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return new EmptyGameResult();
        }

        public async Task<EmptyGameResult> Unload([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                companyStorageId = default(long),
                contractId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var truck = await ctx.Trucks.FindAsync(d.modelId);
            if (truck == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var companyStorage = await ctx.CompanyStorages.FindAsync(d.companyStorageId);
            if (companyStorage == null) return Errors.DoesNotExist(d.modelId, localization.Storages.CompanyStorage);

            var contract = await ctx.CompanyContracts.FindAsync(d.contractId);
            if (contract == null) return Errors.DoesNotExist(d.contractId, localization.Business.Company.CompanyContract);


            var actionResult = truck.Unload(companyStorage, contract);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return new EmptyGameResult();
        }

        public async Task<GameResult<uint>> SimpleUpgrade([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                invokerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var truck = await ctx.Trucks.FindAsync(d.modelId);
            if (truck == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);


            var actionResult = truck.Upgrade(gameSettings, invoker);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return truck.DeliveringSpeedInSeconds;
        }

        public async Task<GameResult<uint>> Upgrade([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                stepInSeconds = default(uint),
                invokerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var truck = await ctx.Trucks.FindAsync(d.modelId);
            if (truck == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);


            var actionResult = truck.Upgrade(d.stepInSeconds, gameSettings, invoker);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return truck.DeliveringSpeedInSeconds;
        }

        public async Task<GameResult<uint>> ForceUpgrade([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                stepInSeconds = default(uint)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var truck = await ctx.Trucks.FindAsync(d.modelId);
            if (truck == null) return Errors.DoesNotExist(d.modelId, modelLocalization);


            var actionResult = truck.Upgrade(d.stepInSeconds, gameSettings);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return truck.DeliveringSpeedInSeconds;
        }

        public async Task<GameResult<decimal>> GetSpaceOccupiedByItemsForContract([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                contractId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var truck = await ctx.Trucks.FindAsync(d.modelId);
            if (truck == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var contract = await ctx.CompanyContracts.FindAsync(d.contractId);
            if (contract == null) return Errors.DoesNotExist(d.contractId, localization.Business.Company.CompanyContract);


            var actionResult = truck.GetSpaceOccupiedByItemsForContract(contract);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        //public async Task<EmptyGameResult> ScheduleUnloadTruck([FromBody] string json, [FromServices] IBackgroundJobScheduler jobs)
        //{
        //    var pars = new
        //    {
        //        modelId = default(long),
        //        companyStorageId = default(long),
        //        contractId = default(long)
        //    };

        //    var d = JsonConvert.DeserializeAnonymousType(json, pars);

        //    var truck = await ctx.Trucks.FindAsync(d.modelId);
        //    if (truck == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

        //    var companyStorage = await ctx.CompanyStorages.FindAsync(d.companyStorageId);
        //    if (companyStorage == null) return Errors.DoesNotExist(d.modelId, localization.Storages.CompanyStorage);

        //    var contract = await ctx.CompanyContracts.FindAsync(d.contractId);
        //    if (contract == null) return Errors.DoesNotExist(d.contractId, localization.Business.Company.CompanyContract);


        //    ScheduleUnloadTruckJob(truck, companyStorage, contract, jobs);

        //    await ctx.SaveChangesAsync();

        //    return new EmptyGameResult();
        //}

        public async Task<EmptyGameResult> ScheduleReturnTruck([FromBody] string json, [FromServices] IBackgroundJobScheduler jobs)
        {
            var pars = new
            {
                modelId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var truck = await ctx.Trucks.FindAsync(d.modelId);
            if (truck == null) return Errors.DoesNotExist(d.modelId, modelLocalization);


            ScheduleReturnTruckJob(truck, jobs);

            await ctx.SaveChangesAsync();

            return new EmptyGameResult();
        }


        private void ScheduleUnloadTruckJob(Truck truck, CompanyStorage companyStorage, CompanyContract contract, IBackgroundJobScheduler jobs)
        {
            string jobId = jobs.Schedule<UnloadTruckJob, UnloadTruckJobParams>(new UnloadTruckJobParams
            {
                TruckId = truck.Id,
                CompanyStorageId = companyStorage.Id,
                ContractId = contract.Id
            }, TimeSpan.FromSeconds(truck.DeliveringSpeedInSeconds / 2));

            truck.UnloadTruckJobId = jobId;
        }

        private void ScheduleReturnTruckJob(Truck truck, IBackgroundJobScheduler jobs)
        {
            string jobId = jobs.Schedule<IReturnTruckJob, ReturnTruckJobParams>(new ReturnTruckJobParams
            {
                TruckId = truck.Id
            }, TimeSpan.FromSeconds(truck.DeliveringSpeedInSeconds / 2));
        }
    }
}
