using EnterpriseBot.Api.Abstractions;
using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Game.Localization;
using EnterpriseBot.Api.Models.ApiCreationParams.Crafting;
using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.Contexts;
using EnterpriseBot.Api.Models.ModelCreationParams.Crafting;
using EnterpriseBot.Api.Models.Other;
using EnterpriseBot.Api.Models.Settings;
using EnterpriseBot.Api.Models.Settings.LocalizationSettings;
using EnterpriseBot.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Areas.Crafting.Controllers
{
    [Area(nameof(Crafting))]
    public class ItemController : Controller,
                                  IGameController<Item, ItemApiCreationParams>
    {
        private readonly ApplicationContext ctx;
        private readonly ILogger<ItemController> logger;

        private readonly GameSettings gameSettings;

        private readonly LocalizationSettings localization;
        private readonly LocalizationSetting modelLocalization;

        public ItemController(ApplicationContext dbContext,
                              ILogger<ItemController> logger,
                              IOptionsSnapshot<GameSettings> gameOptionsAccessor)
        {
            this.ctx = dbContext;
            this.logger = logger;

            this.gameSettings = gameOptionsAccessor.Value;

            this.localization = this.gameSettings.Localization;
            this.modelLocalization = this.localization.Crafting.Item;
        }

        ///<inheritdoc/>
        public async Task<GameResult<Item>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            var model = await ctx.Items.FindAsync(id);

            return model;
        }

        ///<inheritdoc/>
        public async Task<GameResult<Item>> Create([FromBody] ItemApiCreationParams pars)
        {
            var name = await ctx.LocalizedStrings.FindAsync(pars.NameLocalizedStringId);
            if (name == null) return Errors.DoesNotExist(pars.NameLocalizedStringId, localization.Localization.LocalizedString);

            var craftingSubCategory = await ctx.CraftingSubCategories.FindAsync(pars.CraftingSubCategoryId);
            if (craftingSubCategory == null) return Errors.DoesNotExist(pars.CraftingSubCategoryId, localization.Crafting.CraftingSubCategory);

            var creationResult = Item.Create(new ItemCreationParams
            {
                Name = name,
                Category = craftingSubCategory,
                Space = pars.Space
            }, gameSettings);
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.Items.Add(model);
            await ctx.SaveChangesAsync();

            return model;
        }

        public async Task<GameResult<StringLocalization>> EditName([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                newName = default(string),
                language = default(LocalizationLanguage)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var item = await ctx.Items.FindAsync(d.modelId);
            if (item == null) return Errors.DoesNotExist(d.modelId, localization.Crafting.Item);

            return item.EditName(d.newName, d.language, gameSettings);
        }

        public async Task<GameResult<decimal>> SetSpace([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                newSpace = default(decimal)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var item = await ctx.Items.FindAsync(d.modelId);
            if(item == null) return Errors.DoesNotExist(d.modelId, localization.Crafting.Item);

            return item.SetSpace(d.newSpace);
        }

        public async Task<GameResult<CraftingSubCategory>> SetCategory([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                newCraftingSubCategoryId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var item = await ctx.Items.FindAsync(d.modelId);
            if (item == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var newCategory = await ctx.CraftingSubCategories.FindAsync(d.newCraftingSubCategoryId);
            if (newCategory == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            return item.SetCategory(newCategory);
        }
    }
}
