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
    public class ShowcaseStorageController : Controller,
                                              IGameController<ShowcaseStorage, ShowcaseStorageApiCreationParams>
    {
        private readonly ApplicationContext ctx;
        private readonly ILogger<ShowcaseStorageController> logger;

        private readonly GameSettings gameSettings;

        private readonly LocalizationSettings localization;
        private readonly LocalizationSetting modelLocalization;

        public ShowcaseStorageController(ApplicationContext dbContext,
                                          ILogger<ShowcaseStorageController> logger,
                                          IOptionsSnapshot<GameSettings> gameOptionsAccessor)
        {
            this.ctx = dbContext;
            this.logger = logger;

            this.gameSettings = gameOptionsAccessor.Value;

            this.localization = this.gameSettings.Localization;
            this.modelLocalization = this.localization.Storages.ShowcaseStorage;
        }

        ///<inheritdoc/>
        public async Task<GameResult<ShowcaseStorage>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            var model = await ctx.ShowcaseStorages.FindAsync(id);

            return model;
        }

        public async Task<GameResult<ShowcaseStorage>> Create([FromBody] ShowcaseStorageApiCreationParams pars)
        {
            var company = await ctx.Companies.FindAsync(pars.OwningCompanyId);
            if (company == null) return Errors.DoesNotExist(pars.OwningCompanyId, localization.Business.Company.Company);

            var creationResult = ShowcaseStorage.Create(new ShowcaseStorageCreationParams
            {
                OwningCompany = company,
                Capacity = pars.Capacity
            });
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.ShowcaseStorages.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }


        public async Task<GameResult<ShowcaseStorage>> BuyAndCreate([FromBody] string json)
        {
            var pars = new
            {
                companyId = default(long),
                showcaseStorageCreationParams = default(ShowcaseStorageApiCreationParams),
                invokerId = default(long?)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var creationPars = d.showcaseStorageCreationParams;

            var company = await ctx.Companies.FindAsync(d.companyId);
            if (company == null) return Errors.DoesNotExist(d.companyId, localization.Business.Company.Company);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);

            var buyResult = ShowcaseStorage.Buy(company, gameSettings, invoker);
            if (buyResult.LocalizedError != null) return buyResult.LocalizedError;

            var creationResult = ShowcaseStorage.Create(new ShowcaseStorageCreationParams
            {
                Capacity = creationPars.Capacity,
                OwningCompany = company
            });
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.ShowcaseStorages.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }
        

        public async Task<GameResult<ItemPrice>> AddPrice([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                itemId = default(long),
                price = default(decimal),
                invokerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);
            var showcaseStorage = await ctx.ShowcaseStorages.FindAsync(d.modelId);
            if (showcaseStorage == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var item = await ctx.Items.FindAsync(d.itemId);
            if (item == null) return Errors.DoesNotExist(d.itemId, localization.Crafting.Item);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);


            var actionResult = showcaseStorage.AddPrice(item, d.price, invoker);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<ItemPrice>> SetPrice([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                itemId = default(long),
                price = default(decimal),
                invokerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var showcaseStorage = await ctx.ShowcaseStorages.FindAsync(d.modelId);
            if (showcaseStorage == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var item = await ctx.Items.FindAsync(d.itemId);
            if (item == null) return Errors.DoesNotExist(d.itemId, localization.Crafting.Item);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);


            var actionResult = showcaseStorage.SetPrice(item, d.price, invoker);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<ItemPrice>> GetPrice([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                itemId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var showcaseStorage = await ctx.ShowcaseStorages.FindAsync(d.modelId);
            if (showcaseStorage == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var item = await ctx.Items.FindAsync(d.itemId);
            if (item == null) return Errors.DoesNotExist(d.itemId, localization.Crafting.Item);


            var actionResult = showcaseStorage.GetPrice(item);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<bool>> IsPriceDefinedForItem([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                itemId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var showcaseStorage = await ctx.ShowcaseStorages.FindAsync(d.modelId);
            if (showcaseStorage == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var item = await ctx.Items.FindAsync(d.itemId);
            if (item == null) return Errors.DoesNotExist(d.itemId, localization.Crafting.Item);

            return showcaseStorage.IsPriceDefinedForItem(item);
        }

        public async Task<GameResult<int>> BuyItem([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                itemId = default(long),
                quantity = default(int),
                buyerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var showcaseStorage = await ctx.ShowcaseStorages.FindAsync(d.modelId);
            if (showcaseStorage == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var item = await ctx.Items.FindAsync(d.itemId);
            if (item == null) return Errors.DoesNotExist(d.itemId, localization.Crafting.Item);

            var buyer = await ctx.Players.FindAsync(d.buyerId);
            if (buyer == null) return Errors.DoesNotExist(d.buyerId, localization.Essences.Player);


            var actionResult = showcaseStorage.BuyItem(item, d.quantity, buyer);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<decimal>> UpgradeCapacity([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                invokerId = default(long?)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var showcaseStorage = await ctx.ShowcaseStorages.FindAsync(d.modelId);
            if (showcaseStorage == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var invoker = await ctx.Players.FindAsync(d.invokerId);

            var actionResult = showcaseStorage.UpgradeCapacity(gameSettings, invoker);
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

            var showcaseStorage = await ctx.ShowcaseStorages.FindAsync(d.modelId);
            if (showcaseStorage == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);

            return showcaseStorage.HasPermissionToManage(invoker);
        }

        public async Task<GameResult<bool>> HasPermissionToManagePrices([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                invokerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var showcaseStorage = await ctx.ShowcaseStorages.FindAsync(d.modelId);
            if (showcaseStorage == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);

            return showcaseStorage.HasPermissionToManagePrices(invoker);
        }

        public async Task<EmptyGameResult> ReturnErrorIfDoesNotHavePermissionToManage([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                invokerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var showcaseStorage = await ctx.ShowcaseStorages.FindAsync(d.modelId);
            if (showcaseStorage == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);

            return showcaseStorage.ReturnErrorIfDoesNotHavePermissionToManage(invoker);
        }

        public async Task<EmptyGameResult> ReturnErrorIfDoesNotHavePermissionToManagePrices([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                invokerId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var showcaseStorage = await ctx.ShowcaseStorages.FindAsync(d.modelId);
            if (showcaseStorage == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var invoker = await ctx.Players.FindAsync(d.invokerId);
            if (invoker == null) return Errors.DoesNotExist(d.invokerId, localization.Essences.Player);

            return showcaseStorage.ReturnErrorIfDoesNotHavePermissionToManagePrices(invoker);
        }
    }
}
