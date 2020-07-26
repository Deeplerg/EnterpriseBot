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
using EnterpriseBot.BackgroundJobs.Jobs;
using EnterpriseBot.BackgroundJobs.Params;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Areas.Business.Company.Controllers
{
    [Area(nameof(Business))]
    [SubArea(nameof(Business.Company))]
    public class TruckGarageController : Controller,
                                         IGameController<TruckGarage, TruckGarageApiCreationParams>
    {
        private readonly ApplicationContext ctx;
        private readonly ILogger<TruckGarageController> logger;

        private readonly GameSettings gameSettings;

        private readonly LocalizationSettings localization;
        private readonly LocalizationSetting modelLocalization;

        public TruckGarageController(ApplicationContext dbContext,
                                     ILogger<TruckGarageController> logger,
                                     IOptionsSnapshot<GameSettings> gameOptionsAccessor)
        {
            this.ctx = dbContext;
            this.logger = logger;

            this.gameSettings = gameOptionsAccessor.Value;

            this.localization = this.gameSettings.Localization;
            this.modelLocalization = this.localization.Business.Company.TruckGarage;
        }

        ///<inheritdoc/>
        public async Task<GameResult<TruckGarage>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            var model = await ctx.TruckGarages.FindAsync(id);

            return model;
        }

        public async Task<GameResult<TruckGarage>> Create([FromBody] TruckGarageApiCreationParams pars)
        {
            var company = await ctx.Companies.FindAsync(pars.OwningCompanyId);
            if (company == null) return Errors.DoesNotExist(pars.OwningCompanyId, localization.Business.Company.Company);

            var creationResult = TruckGarage.Create(new TruckGarageCreationParams
            {
                Capacity = pars.Capacity,
                OwningCompany = company
            }, gameSettings);
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult;

            ctx.TruckGarages.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }

        public async Task<GameResult<Truck>> AddTruck([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                truckCreationParams = default(TruckApiCreationParams)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var truckPars = d.truckCreationParams;

            var truckGarage = await ctx.TruckGarages.FindAsync(d.modelId);
            if (truckGarage == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = truckGarage.AddTruck(new TruckCreationParams
            {
                TruckGarage = truckGarage,
                Capacity = truckPars.Capacity,
                DeliveringSpeedInSeconds = truckPars.DeliveringSpeedInSeconds
            });
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            var model = actionResult.Result;

            await ctx.SaveChangesAsync();

            return model;
        }

        public async Task<GameResult<Truck>> BuyAndAddTruck([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                truckCreationParams = default(TruckApiCreationParams),
                invokerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var truckPars = d.truckCreationParams;

            var truckGarage = await ctx.TruckGarages.FindAsync(d.modelId);
            if (truckGarage == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);


            var buyResult = truckGarage.BuyTruck(gameSettings, invoker);
            if (buyResult.LocalizedError != null) return buyResult.LocalizedError;

            var addResult = truckGarage.AddTruck(new TruckCreationParams
            {
                TruckGarage = truckGarage,
                Capacity = truckPars.Capacity,
                DeliveringSpeedInSeconds = truckPars.DeliveringSpeedInSeconds
            });
            if (addResult.LocalizedError != null) return addResult.LocalizedError;


            await ctx.SaveChangesAsync();

            return addResult;
        }

        public async Task<GameResult<sbyte>> Upgrade([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                invokerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var truckGarage = await ctx.TruckGarages.FindAsync(d.modelId);
            if (truckGarage == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);


            var actionResult = truckGarage.Upgrade(gameSettings, invoker);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<sbyte>> ForceUpgrade([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                step = default(sbyte)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var truckGarage = await ctx.TruckGarages.FindAsync(d.modelId);
            if (truckGarage == null) return Errors.DoesNotExist(d.modelId, modelLocalization);


            var actionResult = truckGarage.ForceUpgrade(d.step, gameSettings);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }
    }
}
