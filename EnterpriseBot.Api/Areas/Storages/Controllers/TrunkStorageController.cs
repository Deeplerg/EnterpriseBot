using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Game.Storages;
using EnterpriseBot.Api.Models.ApiCreationParams.Storages;
using EnterpriseBot.Api.Models.Contexts;
using EnterpriseBot.Api.Models.ModelCreationParams.Storages;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Areas.Storages.Controllers
{
    [Area(nameof(Storages))]
    public class TrunkStorageController : Controller,
                                          IGameController<TrunkStorage, TrunkStorageApiCreationParams>
    {
        private readonly ApplicationContext ctx;
        private readonly ILogger<TrunkStorageController> logger;

        private readonly GameSettings gameSettings;

        private readonly LocalizationSettings localization;
        private readonly LocalizationSetting modelLocalization;

        public TrunkStorageController(ApplicationContext dbContext,
                                      ILogger<TrunkStorageController> logger,
                                      IOptionsSnapshot<GameSettings> gameOptionsAccessor)
        {
            this.ctx = dbContext;
            this.logger = logger;

            this.gameSettings = gameOptionsAccessor.Value;

            this.localization = this.gameSettings.Localization;
            this.modelLocalization = this.localization.Storages.TrunkStorage;
        }

        ///<inheritdoc/>
        public async Task<GameResult<TrunkStorage>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            var model = await ctx.TrunkStorages.FindAsync(id);

            return model;
        }

        public async Task<GameResult<TrunkStorage>> Create([FromBody] TrunkStorageApiCreationParams pars)
        {
            var truck = await ctx.Trucks.FindAsync(pars.OwningTruckId);
            if (truck == null) return Errors.DoesNotExist(pars.OwningTruckId, localization.Business.Company.Truck);

            var creationResult = TrunkStorage.Create(new TrunkStorageCreationParams
            {
                Capacity = pars.Capacity,
                OwningTruck = truck
            });
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.TrunkStorages.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }


        public async Task<GameResult<decimal>> UpgradeCapacity([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                invokerId = default(long?)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var trunkStorage = await ctx.TrunkStorages.FindAsync(d.modelId);
            if (trunkStorage == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var invoker = await ctx.Players.FindAsync(d.invokerId);

            var actionResult = trunkStorage.UpgradeCapacity(gameSettings, invoker);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            return actionResult;
        }

        public async Task<GameResult<bool>> HasPermissionToManage([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                invokerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var trunkStorage = await ctx.TrunkStorages.FindAsync(d.modelId);
            if (trunkStorage == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);

            return trunkStorage.HasPermissionToManage(invoker);
        }

        public async Task<EmptyGameResult> ReturnErrorIfDoesNotHavePermissionToManage([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                invokerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var trunkStorage = await ctx.TrunkStorages.FindAsync(d.modelId);
            if (trunkStorage == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);

            return trunkStorage.ReturnErrorIfDoesNotHavePermissionToManage(invoker);
        }
    }
}
