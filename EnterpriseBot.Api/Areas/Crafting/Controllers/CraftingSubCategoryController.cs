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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Areas.Crafting.Controllers
{
    [Area(nameof(Crafting))]
    public class CraftingSubCategoryController : Controller,
                                                 IGameController<CraftingSubCategory, CraftingSubCategoryApiCreationParams>
    {
        private readonly ApplicationContext ctx;
        private readonly ILogger<CraftingSubCategoryController> logger;

        private readonly GameSettings gameSettings;

        private readonly LocalizationSettings localization;
        private readonly LocalizationSetting modelLocalization;

        public CraftingSubCategoryController(ApplicationContext dbContext,
                                             ILogger<CraftingSubCategoryController> logger,
                                             IOptionsSnapshot<GameSettings> gameOptionsAccessor)
        {
            this.ctx = dbContext;
            this.logger = logger;

            this.gameSettings = gameOptionsAccessor.Value;

            this.localization = this.gameSettings.Localization;
            this.modelLocalization = this.localization.Crafting.CraftingSubCategory;
        }

        ///<inheritdoc/>
        public async Task<GameResult<CraftingSubCategory>> Get([FromBody] IdParameter idpar)
        {
            long id = (long)idpar.Id;

            var model = await ctx.CraftingSubCategories.FindAsync(id);

            return model;
        }

        ///<inheritdoc/>
        public async Task<GameResult<CraftingSubCategory>> Create([FromBody] CraftingSubCategoryApiCreationParams pars)
        {
            var name = await ctx.LocalizedStrings.FindAsync(pars.NameLocalizedStringId);
            if (name == null) return Errors.DoesNotExist(pars.NameLocalizedStringId, localization.Localization.LocalizedString);

            var description = await ctx.LocalizedStrings.FindAsync(pars.DescriptionLocalizedStringId);
            if (description == null) return Errors.DoesNotExist(pars.DescriptionLocalizedStringId, localization.Localization.LocalizedString);

            var mainCategory = await ctx.CraftingCategories.FindAsync(pars.MainCraftingCategoryId);
            if (mainCategory == null) return Errors.DoesNotExist(pars.MainCraftingCategoryId, localization.Crafting.CraftingSubCategory);

            List<Item> items = null;

            if (pars.ItemsIds?.Any() is true)
            {
                items = await ctx.Items
                                 .Where(item =>
                                        pars.ItemsIds.Any(id => item.Id == id))
                                 .ToListAsync();
            }

            var creationResult = CraftingSubCategory.Create(new CraftingSubCategoryCreationParams
            {
                Name = name,
                Description = description,
                MainCategory = mainCategory,
                Items = items
            }, gameSettings);
            if (creationResult.LocalizedError != null) return creationResult.LocalizedError;

            var model = creationResult.Result;

            ctx.CraftingSubCategories.Add(model);
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

            var craftingSubCategory = await ctx.CraftingSubCategories.FindAsync(d.modelId);
            if (craftingSubCategory == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = craftingSubCategory.EditName(d.newName, d.language, gameSettings);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<StringLocalization>> EditDescription([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                newDescription = default(string),
                language = default(LocalizationLanguage)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var craftingSubCategory = await ctx.CraftingSubCategories.FindAsync(d.modelId);
            if (craftingSubCategory == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var actionResult = craftingSubCategory.EditDescription(d.newDescription, d.language, gameSettings);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }

        public async Task<GameResult<CraftingCategory>> SetCategory([FromBody] string json)
        {
            var pars = new
            {
                modelId = default(long),
                newCraftingCategoryId = default(long)
            };

            var d = JsonConvert.DeserializeAnonymousType(json, pars);

            var craftingSubCategory = await ctx.CraftingSubCategories.FindAsync(d.modelId);
            if (craftingSubCategory == null) return Errors.DoesNotExist(d.modelId, modelLocalization);

            var newCraftingCategory = await ctx.CraftingCategories.FindAsync(d.newCraftingCategoryId);
            if (newCraftingCategory == null) return Errors.DoesNotExist(d.newCraftingCategoryId, localization.Crafting.CraftingCategory);

            var actionResult = craftingSubCategory.SetCategory(newCraftingCategory);
            if (actionResult.LocalizedError != null) return actionResult.LocalizedError;

            await ctx.SaveChangesAsync();

            return actionResult;
        }
    }
}
