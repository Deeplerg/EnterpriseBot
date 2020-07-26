using Castle.Components.DictionaryAdapter;
using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Game.Essences;
using EnterpriseBot.Api.Game.Storages;
using EnterpriseBot.Api.Models.ApiCreationParams.Storages;
using EnterpriseBot.Api.Models.Common.Enums;
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
    public class CompanyStorageController : Controller,
                                            IGameController<CompanyStorage, CompanyStorageApiCreationParams>
    {
        private readonly ApplicationContext ctx;
        private readonly ILogger<CompanyStorageController> logger;

        private readonly GameSettings gameSettings;

        private readonly LocalizationSettings localization;
        private readonly LocalizationSetting modelLocalization;

        public CompanyStorageController(ApplicationContext dbContext,
                                        ILogger<CompanyStorageController> logger,
                                        IOptionsSnapshot<GameSettings> gameOptionsAccessor)
        {
            this.ctx = dbContext;
            this.logger = logger;

            this.gameSettings = gameOptionsAccessor.Value;

            this.localization = this.gameSettings.Localization;
            this.modelLocalization = this.localization.Storages.CompanyStorage;
        }

        ///<inheritdoc/>
        public async Task<GameResult<CompanyStorage>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            var model = await ctx.CompanyStorages.FindAsync(id);

            return model;
        }

        public async Task<GameResult<CompanyStorage>> Create([FromBody] CompanyStorageApiCreationParams pars)
        {
            var company = await ctx.Companies.FindAsync(pars.OwningCompanyId);
            if (company == null) return Errors.DoesNotExist(pars.OwningCompanyId, localization.Business.Company.Company);

            var creationResult = CompanyStorage.Create(new CompanyStorageCreationParams
            {
                OwningCompany = company,
                Capacity = pars.Capacity,
                Type = pars.Type
            });
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.CompanyStorages.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }
    
        public async Task<GameResult<CompanyStorage>> BuyAndCreate([FromBody] string json)
        {
            var pars = new
            {
                companyId = default(long),
                companyStorageCreationParams = default(CompanyStorageApiCreationParams),
                invokerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var creationPars = d.companyStorageCreationParams;

            var company = await ctx.Companies.FindAsync(d.companyId);
            if (company == null) return Errors.DoesNotExist(d.companyId, localization.Business.Company.Company);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);

            var buyResult = CompanyStorage.Buy(company, creationPars.Type, gameSettings, invoker);
            if (buyResult.LocalizedError != null) return buyResult.LocalizedError;

            var creationResult = CompanyStorage.Create(new CompanyStorageCreationParams
            {
                Capacity = creationPars.Capacity,
                OwningCompany = company,
                Type = creationPars.Type
            });
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.CompanyStorages.Add(model);
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

            var companyStorage = await ctx.CompanyStorages.FindAsync(d.modelId);
            if (companyStorage == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var invoker = await ctx.Players.FindAsync(d.invokerId);


            var actionResult = companyStorage.UpgradeCapacity(gameSettings, invoker);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

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

            var companyStorage = await ctx.CompanyStorages.FindAsync(d.modelId);
            if (companyStorage == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);

            return companyStorage.HasPermissionToManage(invoker);
        }

        public async Task<EmptyGameResult> ReturnErrorIfDoesNotHavePermissionToManage([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                invokerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var companyStorage = await ctx.CompanyStorages.FindAsync(d.modelId);
            if (companyStorage == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);

            return companyStorage.ReturnErrorIfDoesNotHavePermissionToManage(invoker);
        }
    }
}
